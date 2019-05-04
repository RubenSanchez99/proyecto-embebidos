using System;
using System.IO;
using Catalog.API.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Catalog.FunctionalTests
{
    public class DatabaseFixture : IDisposable
    {
        private readonly DbContextOptions<CatalogContext> _options;

        public DatabaseFixture()
        {
            _options = new DbContextOptionsBuilder<CatalogContext>()
                .UseNpgsql("Server=127.0.0.1;Port=5432;Database=CapacitacionMicroservicios.CatalogDb.Test;User Id=postgres;Password=pass;")
                .Options;

            var db = new CatalogContext(_options);
            db.Database.EnsureCreated();

            var sqlFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.sql");

            foreach(string file in sqlFiles)
            {
                db.Database.ExecuteSqlCommand(File.ReadAllText(file));
            }

            db.SaveChanges();
        }

        public void Dispose()
        {
            var db = new CatalogContext(_options);
            db.Database.EnsureDeleted();
        }
    }
}