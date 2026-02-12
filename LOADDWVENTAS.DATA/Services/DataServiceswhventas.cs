using System;
using System.Collections.Generic;
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
                await _dwhContext.Database.ExecuteSqlRawAsync("DELETE FROM Dim_Tiempo");
                await _dwhContext.Database.ExecuteSqlRawAsync("DELETE FROM Dim_Cliente");
                await _dwhContext.Database.ExecuteSqlRawAsync("DELETE FROM Dim_Producto");
                await _dwhContext.Database.ExecuteSqlRawAsync("DELETE FROM Dim_Empleado");
                await _dwhContext.Database.ExecuteSqlRawAsync("DELETE FROM Dim_Shipper");

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
            var existingIds = new HashSet<string>(
                await _dwhContext.DimCliente.AsNoTracking()
                    .Select(c => c.CustomerID)
                    .ToListAsync());

            var clientes = await _northwindContext.Customers
                .AsNoTracking()
                .Select(cliente => new DimCliente
                {
                    CustomerID = cliente.CustomerID ?? "Unknown",
                    CompanyName = cliente.CompanyName ?? "N/A",
                    Country = cliente.Country ?? "Unknown"
                })
                .ToListAsync();

            var nuevos = clientes
                .Where(c => !existingIds.Contains(c.CustomerID))
                .ToList();

            if (nuevos.Count == 0)
            {
                _logger.LogInformation("Clientes: sin nuevos registros.");
                return;
            }

            _dwhContext.DimCliente.AddRange(nuevos);
            await _dwhContext.SaveChangesAsync();
            _logger.LogInformation("Clientes cargados con éxito: {count}.", nuevos.Count);
        }

        public async Task LoadProductosAsync()
        {
            var existingIds = new HashSet<int>(
                await _dwhContext.DimProducto.AsNoTracking()
                    .Select(p => p.ProductID)
                    .ToListAsync());

            var productos = await _northwindContext.Products
                .AsNoTracking()
                .Join(_northwindContext.Categories.AsNoTracking(), p => p.CategoryID, c => c.CategoryID, (p, c) => new { p, c })
                .Select(item => new DimProducto
                {
                    ProductID = item.p.ProductID,
                    ProductName = item.p.ProductName ?? "Unnamed",
                    CategoryID = item.c.CategoryID,
                    CategoryName = item.c.CategoryName ?? "Uncategorized",
                    UnitPrice = item.p.UnitPrice ?? 0.0m
                })
                .ToListAsync();

            var nuevos = productos
                .Where(p => !existingIds.Contains(p.ProductID))
                .ToList();

            if (nuevos.Count == 0)
            {
                _logger.LogInformation("Productos: sin nuevos registros.");
                return;
            }

            _dwhContext.DimProducto.AddRange(nuevos);
            await _dwhContext.SaveChangesAsync();
            _logger.LogInformation("Productos cargados con éxito: {count}.", nuevos.Count);
        }

        public async Task LoadEmpleadosAsync()
        {
            var existingIds = new HashSet<int>(
                await _dwhContext.DimEmpleado.AsNoTracking()
                    .Select(e => e.EmployeeID)
                    .ToListAsync());

            var empleados = await _northwindContext.Employees
                .AsNoTracking()
                .Select(empleado => new DimEmpleado
                {
                    EmployeeID = empleado.EmployeeID,
                    LastName = empleado.LastName,
                    FirstName = empleado.FirstName
                })
                .ToListAsync();

            var nuevos = empleados
                .Where(e => !existingIds.Contains(e.EmployeeID))
                .ToList();

            if (nuevos.Count == 0)
            {
                _logger.LogInformation("Empleados: sin nuevos registros.");
                return;
            }

            _dwhContext.DimEmpleado.AddRange(nuevos);
            await _dwhContext.SaveChangesAsync();
            _logger.LogInformation("Empleados cargados con éxito: {count}.", nuevos.Count);
        }

        public async Task LoadShippersAsync()
        {
            var existingIds = new HashSet<int>(
                await _dwhContext.DimShipper.AsNoTracking()
                    .Select(s => s.ShipperID)
                    .ToListAsync());

            var shippers = await _northwindContext.Shippers
                .AsNoTracking()
                .Select(shipper => new DimShipper
                {
                    ShipperID = shipper.ShipperID,
                    CompanyName = shipper.CompanyName,
                    Phone = shipper.Phone ?? "N/A"
                })
                .ToListAsync();

            var nuevos = shippers
                .Where(s => !existingIds.Contains(s.ShipperID))
                .ToList();

            if (nuevos.Count == 0)
            {
                _logger.LogInformation("Shippers: sin nuevos registros.");
                return;
            }

            _dwhContext.DimShipper.AddRange(nuevos);
            await _dwhContext.SaveChangesAsync();
            _logger.LogInformation("Shippers cargados con éxito: {count}.", nuevos.Count);
        }

        public async Task LoadVentasAsync()
        {
            var tiempoMap = await _dwhContext.DimTiempo.AsNoTracking()
                .ToDictionaryAsync(t => t.Fecha, t => t.TiempoID);

            var existingKeys = await _dwhContext.Fact_Ventas.AsNoTracking()
                .Select(f => new { f.OrderID, f.ProductID })
                .ToListAsync();

            var existingKeySet = new HashSet<(int OrderID, int ProductID)>(
                existingKeys.Select(k => (k.OrderID, k.ProductID)));

            var ventas = await _northwindContext.OrderDetails.AsNoTracking()
                .Join(_northwindContext.Orders.AsNoTracking(), od => od.OrderID, o => o.OrderID, (od, o) => new { od, o })
                .ToListAsync();

            var nuevos = new List<Fact_Ventas>();

            foreach (var venta in ventas)
            {
                if (existingKeySet.Contains((venta.o.OrderID, venta.od.ProductID)))
                {
                    continue;
                }

                if (venta.o.OrderDate == null || !tiempoMap.TryGetValue(venta.o.OrderDate.Value, out var tiempoID))
                {
                    _logger.LogWarning("No se encontró TiempoID para la fecha {0} de OrderID {1}.", venta.o.OrderDate, venta.o.OrderID);
                    continue;
                }

                nuevos.Add(new Fact_Ventas
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
                });
            }

            if (nuevos.Count == 0)
            {
                _logger.LogInformation("Ventas: sin nuevos registros.");
                return;
            }

            _dwhContext.Fact_Ventas.AddRange(nuevos);
            await _dwhContext.SaveChangesAsync();
            _logger.LogInformation("Ventas cargadas con éxito: {count}.", nuevos.Count);
        }

        public async Task LoadTiempoAsync()
        {
            var existingFechas = new HashSet<DateTime>(
                await _dwhContext.DimTiempo.AsNoTracking()
                    .Select(t => t.Fecha)
                    .ToListAsync());

            var fechas = await _northwindContext.Orders.AsNoTracking()
                .Where(o => o.OrderDate.HasValue)
                .Select(o => o.OrderDate!.Value)
                .Distinct()
                .ToListAsync();

            var nuevos = new List<DimTiempo>();

            foreach (var fecha in fechas)
            {
                if (existingFechas.Contains(fecha))
                {
                    continue;
                }

                nuevos.Add(new DimTiempo
                {
                    Fecha = fecha,
                    Año = fecha.Year,
                    Mes = fecha.Month
                });
            }

            if (nuevos.Count == 0)
            {
                _logger.LogInformation("DimTiempo: sin nuevos registros.");
                return;
            }

            _dwhContext.DimTiempo.AddRange(nuevos);
            await _dwhContext.SaveChangesAsync();
            _logger.LogInformation("Dimensión de Tiempo cargada con éxito: {count}.", nuevos.Count);
        }

        public async Task LoadClienteAtendidoAsync()
        {
            var tiempoMap = await _dwhContext.DimTiempo.AsNoTracking()
                .ToDictionaryAsync(t => t.Fecha, t => t.TiempoID);

            var existingOrderIds = new HashSet<int>(
                await _dwhContext.Fact_ClienteAtendido.AsNoTracking()
                    .Select(f => f.OrderID)
                    .ToListAsync());

            var clientesAtendidos = await _northwindContext.Orders.AsNoTracking()
                .Join(_northwindContext.Customers.AsNoTracking(), o => o.CustomerID, c => c.CustomerID, (o, c) => new { o, c })
                .ToListAsync();

            var nuevos = new List<Fact_ClienteAtendido>();

            foreach (var item in clientesAtendidos)
            {
                if (existingOrderIds.Contains(item.o.OrderID))
                {
                    continue;
                }

                if (item.o.OrderDate == null || !tiempoMap.TryGetValue(item.o.OrderDate.Value, out var tiempoID))
                {
                    _logger.LogWarning("No se encontró TiempoID para la fecha {0} de OrderID {1}.", item.o.OrderDate, item.o.OrderID);
                    continue;
                }

                nuevos.Add(new Fact_ClienteAtendido
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
                });
            }

            if (nuevos.Count == 0)
            {
                _logger.LogInformation("ClienteAtendido: sin nuevos registros.");
                return;
            }

            _dwhContext.Fact_ClienteAtendido.AddRange(nuevos);
            await _dwhContext.SaveChangesAsync();
            _logger.LogInformation("Hechos de Cliente Atendido cargados con éxito: {count}.", nuevos.Count);
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
