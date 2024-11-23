using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LoadDWVentas.Data.Context;
using LoadDWVentas.Data.Interfaces;
using LoadDWVentas.Data.Services;

namespace LOADDWVENTAS.WorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();

                    // Registrar DbContexts para Northwind y DWHNorthwindOrders
                    services.AddDbContext<Northwind>();
                    services.AddDbContext<DWHNorthwindOrders>();

                    // Registrar DataServiceswhventas como el servicio de datos
                    services.AddScoped<IDataServiceswhventas, DataServiceswhventas>();
                });
    }
}
