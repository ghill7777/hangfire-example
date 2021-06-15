using System;
using System.Threading;
using System.Threading.Tasks;
using MyStore.Business.Data.Entities;

namespace MyStore.Business.Orders
{
    public class OrderConfirmation : IOrderConfirmation 
    {
        private static Random _random;
        private static Random Random => _random ??= new Random(DateTime.Now.Millisecond);

        public async Task Confirm(Order order)
        {
            await Task.Run(() =>
            {
                var latency = Random.Next(1, 5);
                Thread.Sleep(latency * 1000);
            });
        }
    }

    public interface IOrderConfirmation
    {
        Task Confirm(Order order);
    }
}
