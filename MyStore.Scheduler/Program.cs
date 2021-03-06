using System;
using System.Linq;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using MyStore.Business.Data;
using Microsoft.Extensions.DependencyInjection;
using MyStore.Business.Data.Entities;
using Hangfire;
using Hangfire.SqlServer;
using MyStore.Business.Orders;

namespace MyStore.Scheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigureHangfire();
            RunHangfireServer();
        }

        private static void RunHangfireServer()
        {
            Log("Hangfire server running...");
            using (var server = new BackgroundJobServer())
            {
                Console.ReadLine();
            }
        }

        public static void ArchiveOrders()
        {
            Log("Started ArchiveOrders...");
            var provider = SetupServiceProvider();
            using var scope = provider.CreateScope();
            var context = scope.ServiceProvider.GetService<StoreDbContext>();
            if (context == null) throw new Exception("Unable to resolve dbcontext.");

            var eligibleOrders = context.Orders
                .Where(c => c.ConfirmationDate.HasValue && c.PrintDate.HasValue)
                .ToArray();

            for (var i = eligibleOrders.Length - 1; i >= 0; i--)
            {
                var order = eligibleOrders[i];
                var history = new OrderHistory
                {
                    ArchiveDate = DateTime.UtcNow,
                    ConfirmationDate = order.ConfirmationDate,
                    CustomerName = order.CustomerName,
                    Email = order.Email,
                    Items = order.Items,
                    PrintDate = order.PrintDate
                };

                context.OrderHistories.Add(history);
                context.Orders.Remove(order);
                context.SaveChanges();
            }

            Log("Ended ArchiveOrders...");
        }

        private static void ConfigureHangfire()
        {
            const string hangfireConnectionString = "server=.,1431;uid=sa;pwd=lionsNeverSleep9@;database=Hangfire;";
            GlobalConfiguration.Configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(hangfireConnectionString, new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    UsePageLocksOnDequeue = true,
                    DisableGlobalLocks = true
                })
                .UseActivator(new MyJobActivator(SetupServiceProvider()))
                ;
            RecurringJob.AddOrUpdate("ArchiveOrders", () => ArchiveOrders(), Cron.Minutely);
        }

        private static IServiceProvider SetupServiceProvider()
        {
            const string storeConnectionString = "server=.,1432;uid=sa;pwd=dolphin7!;database=MyStore;";

            var services = new ServiceCollection();
            services.AddDbContext<StoreDbContext>(builder => builder.UseSqlServer(storeConnectionString));
            services.AddTransient<IOrderProcessor, OrderProcessor>().AddTransient<OrderProcessor>();
            services.AddTransient<IOrderConfirmation, OrderConfirmation>().AddTransient<OrderConfirmation>();
            services.AddTransient<IPrintingService, PrintingService>().AddTransient<PrintingService>();
            services.AddTransient<HttpClient>();
            var provider = services.BuildServiceProvider();
            return provider;
        }

        private static void Log(string message)
        {
            Console.WriteLine(message);
        }
    }

    public class MyJobActivator : JobActivator
    {
        private readonly IServiceProvider _provider;

        public MyJobActivator(IServiceProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public override object ActivateJob(Type jobType)
        {
            Console.WriteLine($"Activating type: {jobType.Name}");
            return _provider.GetService(jobType);
        }
    }
}
