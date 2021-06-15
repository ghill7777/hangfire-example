
using Microsoft.EntityFrameworkCore;
using MyStore.Business.Data.Entities;

namespace MyStore.Business.Data
{
    public class StoreDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }

        public StoreDbContext(DbContextOptions<StoreDbContext> options)
            : base(options)
        {
                
        }
    }
}