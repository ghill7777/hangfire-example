using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyStore.Business.Data;
using MyStore.Business.Data.Entities;
using Newtonsoft.Json.Linq;

namespace MyStore.Business.Orders
{
    public class OrderConfirmation : IOrderConfirmation
    {
        private static Random _random;
        private static Random Random => _random ??= new Random(DateTime.Now.Millisecond);
        private readonly StoreDbContext _db;
        private readonly HttpClient _client;

        public OrderConfirmation(StoreDbContext db, HttpClient client)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task Confirm(Order order)
        {
            var latency = Random.Next(3, 5);
            Thread.Sleep(latency * 1000);
            var dbOrder = await _db.Orders.FirstAsync(c => c.Id == order.Id);
            dbOrder.Email = await GetRandomEmail();
            dbOrder.ConfirmationDate = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }

        private async Task<string> GetRandomEmail()
        {
            var json = await _client.GetStringAsync("https://random-data-api.com/api/users/random_user");
            var jToken = JObject.Parse(json);
            var email = jToken.Value<string>("email");
            return email;
        }
    }

    public interface IOrderConfirmation
    {
        Task Confirm(Order order);
    }
}
