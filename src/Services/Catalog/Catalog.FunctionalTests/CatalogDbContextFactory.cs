using System;
using System.Data.Common;
using Catalog.API.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Catalog.FunctionalTests
{
    public class CatalogDbContextFactory : IDisposable
    {
        private DbConnection _connection;

        private DbContextOptions<CatalogContext> CreateOptions()
        {
            return new DbContextOptionsBuilder<CatalogContext>()
                .UseSqlite(_connection).Options;
        }

        public CatalogContext CreateContext()
        {
            if (_connection == null)
            {
                _connection = new SqliteConnection("DataSource=:memory:");
                _connection.Open();

                var options = CreateOptions();
                using (var context = new CatalogContext(options))
                {
                    context.Database.EnsureCreated();
                }
            }

            return new CatalogContext(CreateOptions());
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }
    }
}
