using System;
using System.Threading;
using System.Threading.Tasks;
using MyStore.Business.Data.Entities;

namespace MyStore.Business.Orders
{
    public class OrderProcessor : IOrderProcessor
    {
        private static Random _random;
        private static Random Random => _random ??= new Random(DateTime.Now.Millisecond);
        public async Task<Order> Process(Order order)
        {
            return await Task.Run(() =>
            {
                Thread.Sleep(1000);
                order.Id = Random.Next(0, 9999);
                return order;
            });
        }
    }

    public interface IOrderProcessor
    {
        Task<Order> Process(Order order);
    }
}
