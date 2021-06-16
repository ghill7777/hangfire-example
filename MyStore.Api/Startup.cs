using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MyStore.Business.Data;
using MyStore.Business.Orders;
using Hangfire;
using Hangfire.SqlServer;

namespace MyStore.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: "myCors",
                                builder =>
                                {
                                    builder
                                        .AllowAnyOrigin()
                                        .AllowAnyHeader()
                                        .AllowAnyHeader();
                                });
            });
            services.AddControllers();
            services.AddTransient<IOrderConfirmation, OrderConfirmation>();
            services.AddTransient<IOrderProcessor, OrderProcessor>();
            services.AddTransient<IPrintingService, PrintingService>();
            services.AddTransient<HttpClient>();
            services.AddDbContext<StoreDbContext>(builder => builder.UseSqlServer("server=.,1432;uid=sa;pwd=dolphin7!;database=MyStore",
                b => b.MigrationsAssembly("MyStore.Api")));

            services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage("server=.,1431;uid=sa;pwd=lionsNeverSleep9@;database=Hangfire", new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true,
                    PrepareSchemaIfNecessary = false
                }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("myCors");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            try
            {
                using var db = app.ApplicationServices.CreateScope().ServiceProvider.GetService<StoreDbContext>();
                db?.Database.Migrate();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
