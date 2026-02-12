using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using LoadDWVentas.Data.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LOADDWVENTAS.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;

        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dataService = scope.ServiceProvider.GetRequiredService<IDataServiceswhventas>();

                    try
                    {
                        // Llamadas a los m�todos de carga
                        await dataService.LoadTiempoAsync();
                        _logger.LogInformation("Dimensi�n de Tiempo cargada.");

                        await dataService.LoadClientesAsync();
                        _logger.LogInformation("Dimensi�n de Clientes cargada.");

                        await dataService.LoadProductosAsync();
                        _logger.LogInformation("Dimensi�n de Productos cargada.");

                        await dataService.LoadEmpleadosAsync();
                        _logger.LogInformation("Dimensi�n de Empleados cargada.");

                        await dataService.LoadShippersAsync();
                        _logger.LogInformation("Dimensi�n de Shippers cargada.");

                        await dataService.LoadVentasAsync();
                        _logger.LogInformation("Hechos de Ventas cargados.");

                        await dataService.LoadClienteAtendidoAsync();
                        _logger.LogInformation("Hechos de Cliente Atendido cargados.");

                        // Consultar y registrar datos de las vistas
                        var ventas = await dataService.GetFactVentasAsync();
                        _logger.LogInformation("Datos obtenidos de vwFactVentas: {count} registros.", ventas.Count);

                        var clientesAtendidos = await dataService.GetFactClienteAtendidoAsync();
                        _logger.LogInformation("Datos obtenidos de vwFactClienteAtendido: {count} registros.", clientesAtendidos.Count);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error during data loading or querying process.");
                    }
                }

                // Espera antes de la siguiente ejecuci�n
                await Task.Delay(TimeSpan.FromHours(7), stoppingToken);
            }
        }
    }
}
