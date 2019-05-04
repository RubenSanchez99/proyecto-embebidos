using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Catalog.API;
using Catalog.API.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IO;
using System.Reflection;
using Xunit;

namespace Catalog.FunctionalTests
{
    public class CatalogScenariosBase
    {

        public TestServer CreateServer()
        {
            var path = Assembly.GetAssembly(typeof(CatalogScenariosBase))
              .Location;

            var hostBuilder = new WebHostBuilder()
                .ConfigureAppConfiguration((cb, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false)
                    .AddEnvironmentVariables();
                })
                .UseStartup<Startup>()
                .ConfigureLogging((hostingContext, builder) =>
                {
                    builder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    builder.SetMinimumLevel(LogLevel.Debug);
                });

            var testServer = new TestServer(hostBuilder);

            return testServer;
        }

        public static class Get
        {
            private const int PageIndex = 0;
            private const int PageCount = 4;

            public static string Items(bool paginated = false)
            {
                return paginated
                    ? "api/v1/catalog/items" + Paginated(PageIndex, PageCount)
                    : "api/v1/catalog/items";
            }

            public static string ItemById(int id)
            {
                return $"api/v1/catalog/items/{id}";
            }

            public static string ItemByName(string name, bool paginated = false)
            {
                return paginated
                    ? $"api/v1/catalog/items/withname/{name}" + Paginated(PageIndex, PageCount)
                    : $"api/v1/catalog/items/withname/{name}";
            }

            public static string Types = "api/v1/catalog/catalogtypes";

            public static string Brands = "api/v1/catalog/catalogbrands";

            public static string Filtered(int catalogTypeId, int catalogBrandId, bool paginated = false)
            {
                return paginated
                    ? $"api/v1/catalog/items/type/{catalogTypeId}/brand/{catalogBrandId}" + Paginated(PageIndex, PageCount)
                    : $"api/v1/catalog/items/type/{catalogTypeId}/brand/{catalogBrandId}";
            }

            private static string Paginated(int pageIndex, int pageCount)
            {
                return $"?pageIndex={pageIndex}&pageSize={pageCount}";
            }
        }
    }
}
