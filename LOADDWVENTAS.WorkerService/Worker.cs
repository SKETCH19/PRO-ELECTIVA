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
                        // Llamadas a los métodos de carga
                        await dataService.LoadTiempoAsync();
                        _logger.LogInformation("Dimensión de Tiempo cargada.");

                        await dataService.LoadClientesAsync();
                        _logger.LogInformation("Dimensión de Clientes cargada.");

                        await dataService.LoadProductosAsync();
                        _logger.LogInformation("Dimensión de Productos cargada.");

                        await dataService.LoadEmpleadosAsync();
                        _logger.LogInformation("Dimensión de Empleados cargada.");

                        await dataService.LoadShippersAsync();
                        _logger.LogInformation("Dimensión de Shippers cargada.");

                        await dataService.LoadVentasAsync();
                        _logger.LogInformation("Hechos de Ventas cargados.");

                        await dataService.LoadClienteAtendidoAsync();
                        _logger.LogInformation("Hechos de Cliente Atendido cargados.");

                        // Consultar y registrar datos de las vistas
                        var ventas = await dataService.GetFactVentasAsync();
                        _logger.LogInformation("Datos obtenidos de vwFactVentas: {count} registros.", ventas.Count);

                        var clientesAtendidos = await dataService.GetFactClienteAtendidoAsync();
                        _logger.LogInformation("Datos obtenidos de vwFactClienteAtendido: {count} registros.", clientesAtendidos.Count);

                        // Opcional: realizar otras operaciones con los datos obtenidos
                        foreach (var venta in ventas)
                        {
                            _logger.LogInformation("VentaID: {id}, SaleAmount: {amount}", venta.VentaID, venta.SaleAmount);
                        }

                        foreach (var cliente in clientesAtendidos)
                        {
                            _logger.LogInformation("ClienteAtendidoID: {id}, MontoTransaccion: {monto}", cliente.ClienteAtendidoID, cliente.MontoTransaccion);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error during data loading or querying process.");
                    }
                }

                // Espera antes de la siguiente ejecución
                await Task.Delay(TimeSpan.FromHours(7), stoppingToken);
            }
        }
    }
}
