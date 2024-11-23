using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using LoadDWVentas.Data.Context;
using LoadDWVentas.Data.Interfaces;
using LoadDWVentas.Data.Entities.DwVentas;
using Microsoft.EntityFrameworkCore;

namespace LoadDWVentas.Data.Services
{
    public class DataServiceswhventas : IDataServiceswhventas
    {
        private readonly Northwind _northwindContext;
        private readonly DWHNorthwindOrders _dwhContext;
        private readonly ILogger<DataServiceswhventas> _logger;

        public DataServiceswhventas(Northwind northwindContext, DWHNorthwindOrders dwhContext, ILogger<DataServiceswhventas> logger)
        {
            _northwindContext = northwindContext;
            _dwhContext = dwhContext;
            _logger = logger;
        }

        public async Task LimpiarTablasAsync()
        {
            try
            {
                // Limpia primero las tablas de hechos
                _logger.LogInformation("Iniciando limpieza de tablas del Data Warehouse...");
                await _dwhContext.Database.ExecuteSqlRawAsync("DELETE FROM Fact_Ventas");
                await _dwhContext.Database.ExecuteSqlRawAsync("DELETE FROM Fact_ClienteAtendido");

                // Limpia las tablas de dimensiones
                await _dwhContext.Database.ExecuteSqlRawAsync("DELETE FROM DimTiempo");
                await _dwhContext.Database.ExecuteSqlRawAsync("DELETE FROM DimCliente");
                await _dwhContext.Database.ExecuteSqlRawAsync("DELETE FROM DimProducto");
                await _dwhContext.Database.ExecuteSqlRawAsync("DELETE FROM DimEmpleado");
                await _dwhContext.Database.ExecuteSqlRawAsync("DELETE FROM DimShipper");

                _logger.LogInformation("Limpieza de tablas completada con éxito.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al limpiar las tablas del Data Warehouse: {0}", ex.Message);
                throw;
            }
        }

        public async Task LoadClientesAsync()
        {
            var clientes = _northwindContext.Customers.ToList();
            foreach (var cliente in clientes)
            {
                var dimCliente = new DimCliente
                {
                    CustomerID = cliente.CustomerID ?? "Unknown",
                    CompanyName = cliente.CompanyName ?? "N/A",
                    Country = cliente.Country ?? "Unknown"
                };

                if (!_dwhContext.DimCliente.Any(c => c.CustomerID == dimCliente.CustomerID))
                {
                    _dwhContext.DimCliente.Add(dimCliente);
                }
            }

            await _dwhContext.SaveChangesAsync();
            _logger.LogInformation("Clientes cargados con éxito.");
        }

        public async Task LoadProductosAsync()
        {
            var productos = _northwindContext.Products
                .Join(_northwindContext.Categories, p => p.CategoryID, c => c.CategoryID, (p, c) => new { p, c })
                .ToList();

            foreach (var item in productos)
            {
                var dimProducto = new DimProducto
                {
                    ProductID = item.p.ProductID,
                    ProductName = item.p.ProductName ?? "Unnamed",
                    CategoryID = item.c.CategoryID,
                    CategoryName = item.c.CategoryName ?? "Uncategorized",
                    UnitPrice = item.p.UnitPrice ?? 0.0m
                };

                if (!_dwhContext.DimProducto.Any(p => p.ProductID == dimProducto.ProductID))
                {
                    _dwhContext.DimProducto.Add(dimProducto);
                }
            }

            await _dwhContext.SaveChangesAsync();
            _logger.LogInformation("Productos cargados con éxito.");
        }

        public async Task LoadEmpleadosAsync()
        {
            var empleados = _northwindContext.Employees.ToList();
            foreach (var empleado in empleados)
            {
                var dimEmpleado = new DimEmpleado
                {
                    EmployeeID = empleado.EmployeeID,
                    LastName = empleado.LastName,
                    FirstName = empleado.FirstName
                };

                if (!_dwhContext.DimEmpleado.Any(e => e.EmployeeID == dimEmpleado.EmployeeID))
                {
                    _dwhContext.DimEmpleado.Add(dimEmpleado);
                }
            }

            await _dwhContext.SaveChangesAsync();
            _logger.LogInformation("Empleados cargados con éxito.");
        }

        public async Task LoadShippersAsync()
        {
            var shippers = _northwindContext.Shippers.ToList();
            foreach (var shipper in shippers)
            {
                var dimShipper = new DimShipper
                {
                    ShipperID = shipper.ShipperID,
                    CompanyName = shipper.CompanyName,
                    Phone = shipper.Phone ?? "N/A"
                };

                if (!_dwhContext.DimShipper.Any(s => s.ShipperID == dimShipper.ShipperID))
                {
                    _dwhContext.DimShipper.Add(dimShipper);
                }
            }

            await _dwhContext.SaveChangesAsync();
            _logger.LogInformation("Shippers cargados con éxito.");
        }

        public async Task LoadVentasAsync()
        {
            var ventas = _northwindContext.OrderDetails
                .Join(_northwindContext.Orders, od => od.OrderID, o => o.OrderID, (od, o) => new { od, o })
                .ToList();

            foreach (var venta in ventas)
            {
                var tiempo = _dwhContext.DimTiempo.FirstOrDefault(t => t.Fecha == venta.o.OrderDate);
                int tiempoID = tiempo != null ? tiempo.TiempoID : 0;

                if (tiempoID == 0)
                {
                    _logger.LogWarning("No se encontró TiempoID para la fecha {0} de OrderID {1}.", venta.o.OrderDate, venta.o.OrderID);
                    continue;
                }

                var factVenta = new Fact_Ventas
                {
                    OrderID = venta.o.OrderID,
                    TiempoID = tiempoID,
                    ProductID = venta.od.ProductID,
                    CustomerID = venta.o.CustomerID ?? "Unknown",
                    EmployeeID = venta.o.EmployeeID ?? 0,
                    ShipperID = venta.o.ShipVia ?? 0,
                    Quantity = venta.od.Quantity,
                    UnitPrice = venta.od.UnitPrice,
                    Discount = venta.od.Discount,
                    SaleAmount = venta.od.Quantity * venta.od.UnitPrice * (1 - (decimal)venta.od.Discount)
                };

                _dwhContext.Fact_Ventas.Add(factVenta);
            }

            await _dwhContext.SaveChangesAsync();
            _logger.LogInformation("Ventas cargadas con éxito.");
        }

        public async Task LoadTiempoAsync()
        {
            var fechas = _northwindContext.Orders
                .Where(o => o.OrderDate.HasValue)
                .Select(o => o.OrderDate.GetValueOrDefault())
                .Distinct()
                .ToList();

            foreach (var fecha in fechas)
            {
                if (!_dwhContext.DimTiempo.Any(t => t.Fecha == fecha))
                {
                    var dimTiempo = new DimTiempo
                    {
                        Fecha = fecha,
                        Año = fecha.Year,
                        Mes = fecha.Month
                    };

                    _dwhContext.DimTiempo.Add(dimTiempo);
                }
            }

            await _dwhContext.SaveChangesAsync();
            _logger.LogInformation("Dimensión de Tiempo cargada con éxito.");
        }

        public async Task LoadClienteAtendidoAsync()
        {
            var clientesAtendidos = _northwindContext.Orders
                .Join(_northwindContext.Customers, o => o.CustomerID, c => c.CustomerID, (o, c) => new { o, c })
                .ToList();

            foreach (var item in clientesAtendidos)
            {
                var tiempo = _dwhContext.DimTiempo.FirstOrDefault(t => t.Fecha == item.o.OrderDate);
                int tiempoID = tiempo != null ? tiempo.TiempoID : 0;

                if (tiempoID == 0)
                {
                    _logger.LogWarning("No se encontró TiempoID para la fecha {0} de OrderID {1}.", item.o.OrderDate, item.o.OrderID);
                    continue;
                }

                var factClienteAtendido = new Fact_ClienteAtendido
                {
                    OrderID = item.o.OrderID,
                    CustomerID = item.o.CustomerID ?? "Unknown",
                    EmployeeID = item.o.EmployeeID ?? 0,
                    TiempoID = tiempoID,
                    TiempoRespuesta = (item.o.ShippedDate.HasValue && item.o.OrderDate.HasValue) ?
                       (int)(item.o.ShippedDate.Value - item.o.OrderDate.Value).TotalDays : 0,
                    TiempoResolucion = (item.o.RequiredDate.HasValue && item.o.OrderDate.HasValue) ?
                        (int)(item.o.RequiredDate.Value - item.o.OrderDate.Value).TotalDays : 0,
                    NumeroInteracciones = 1,
                    EstadoFinal = "Completado",
                    RequirioEscalamiento = false,
                    MontoTransaccion = item.o.Freight ?? 0
                };

                _dwhContext.Fact_ClienteAtendido.Add(factClienteAtendido);
            }

            await _dwhContext.SaveChangesAsync();
            _logger.LogInformation("Hechos de Cliente Atendido cargados con éxito.");
        }
        public async Task<List<vwFactVentas>> GetFactVentasAsync()
        {
            try
            {
                _logger.LogInformation("Obteniendo datos de la vista vwFactVentas...");
                var ventas = await _dwhContext.FactVentasView.ToListAsync();
                _logger.LogInformation("Datos obtenidos con éxito.");
                return ventas;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al obtener datos de la vista vwFactVentas: {0}", ex.Message);
                throw;
            }
        }

        public async Task<List<vwFactClienteAtendido>> GetFactClienteAtendidoAsync()
        {
            try
            {
                _logger.LogInformation("Obteniendo datos de la vista vwFactClienteAtendido...");
                var clientesAtendidos = await _dwhContext.ClienteAtendidoView.ToListAsync();
                _logger.LogInformation("Datos obtenidos con éxito.");
                return clientesAtendidos;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al obtener datos de la vista vwFactClienteAtendido: {0}", ex.Message);
                throw;
            }
        }

    }
}
