using System;
using System.Net.Http;
using System.Threading.Tasks;
using MyStore.Business.Data;
using MyStore.Business.Data.Entities;

namespace MyStore.Business.Orders
{
    public class OrderProcessor : IOrderProcessor
    {
        private readonly StoreDbContext _db;

        public OrderProcessor(StoreDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<Order> Process(Order order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            await _db.Orders.AddAsync(order);
            await _db.SaveChangesAsync();
            return order;
        }
    }

    public interface IOrderProcessor
    {
        Task<Order> Process(Order order);
    }
}
