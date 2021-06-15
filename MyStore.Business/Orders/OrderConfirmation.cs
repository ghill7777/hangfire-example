using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyStore.Business.Data;
using MyStore.Business.Data.Entities;

namespace MyStore.Business.Orders
{
    public class OrderConfirmation : IOrderConfirmation 
    {
        private static Random _random;
        private static Random Random => _random ??= new Random(DateTime.Now.Millisecond);
        private readonly StoreDbContext _db;

        public OrderConfirmation(StoreDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task Confirm(Order order)
        {
            var latency = Random.Next(1, 5);
            Thread.Sleep(latency * 1000);
            var dbOrder = await _db.Orders.FirstAsync(c => c.Id == order.Id);
            dbOrder.ConfirmationDate = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
    }

    public interface IOrderConfirmation
    {
        Task Confirm(Order order);
    }
}
