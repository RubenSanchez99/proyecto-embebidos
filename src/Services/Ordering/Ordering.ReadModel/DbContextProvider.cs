using System;
using Microsoft.EntityFrameworkCore;
using EventFlow.EntityFramework;
using Microsoft.Extensions.Configuration;

namespace Ordering.ReadModel
{
    public class DbContextProvider : IDbContextProvider<OrderingDbContext>, IDisposable
    {
        private readonly DbContextOptions<OrderingDbContext> _options;

        public DbContextProvider(string connectionString, IConfiguration config)
        {
            _options = new DbContextOptionsBuilder<OrderingDbContext>()
                .UseNpgsql(config["ConnectionString"])
                .Options;
        }

        public OrderingDbContext CreateContext()
        {
            var context = new OrderingDbContext(_options);
            return context;
        }

        public void Dispose()
        {
        }
    }
}
