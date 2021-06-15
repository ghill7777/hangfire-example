using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MyStore.Business.Data;
using Microsoft.Extensions.DependencyInjection;
using MyStore.Business.Data.Entities;

namespace MyStore.Scheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            var provider = SetupServiceProvider();
            ArchiveOrders(provider);
        }

        private static void ArchiveOrders(IServiceProvider provider)
        {
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
        }

        private static IServiceProvider SetupServiceProvider()
        {
            const string connectionString = "server=.,1432;uid=sa;pwd=dolphin7!;database=MyStore;";
            var services = new ServiceCollection();
            services.AddDbContext<StoreDbContext>(builder => builder.UseSqlServer(connectionString));
            var provider = services.BuildServiceProvider();
            return provider;
        }
    }
}
