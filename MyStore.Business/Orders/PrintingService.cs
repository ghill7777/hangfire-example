using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyStore.Business.Data;
using MyStore.Business.Data.Entities;

namespace MyStore.Business.Orders
{
    public class PrintingService : IPrintingService
    {
        private static Random _random;
        private static Random Random => _random ??= new Random(DateTime.Now.Millisecond);
        private readonly StoreDbContext _db;

        public PrintingService(StoreDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task Print(Order order)
        {
            var latency = Random.Next(3, 5);
            Thread.Sleep(latency * 1000);
            var dbOrder = await _db.Orders.FirstAsync(c => c.Id == order.Id);
            dbOrder.PrintDate = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
    }

    public interface IPrintingService
    {
        Task Print(Order order);
    }
}
