# Power BI guide (Northwind DWH)

This guide assumes SQL Server is running and the database DWHNorthwindOrders already exists.

## 1) Update connection strings
- Edit the connection strings in appsettings.json to match your SQL Server.
- Example for SQL auth:
  - Server=localhost,1433;Database=DWHNorthwindOrders;User ID=sa;Password=YourStrong!Pass;TrustServerCertificate=True;

## 2) Run the SQL setup script
- Open SSMS or Azure Data Studio.
- Run the script:
  - sql/DWHNorthwindOrders_setup.sql
- This creates the required views and adds indexes used by the ETL and Power BI.

## 3) Run the ETL (WorkerService)
- Build and run LOADDWVENTAS.WorkerService.
- Verify row counts in:
  - Fact_Ventas
  - Fact_ClienteAtendido
  - Dim_Tiempo, Dim_Producto, Dim_Cliente, Dim_Empleado, Dim_Shipper

## 4) Connect Power BI to SQL Server
1. Open Power BI Desktop.
2. Get data > SQL Server.
3. Server: <your-server> (example: localhost,1433)
4. Database: DWHNorthwindOrders
5. Data connectivity mode: Import (recommended) or DirectQuery.
6. Select these tables/views:
   - vwFactVentas
   - vwFactClienteAtendido
   - Dim_Tiempo
   - Dim_Producto
   - Dim_Cliente
   - Dim_Empleado
   - Dim_Shipper
   - vwVentasPorMes (optional)
   - vwVentasPorCategoria (optional)
   - vwServicioClientePorMes (optional)

## 5) Create relationships (Model view)
- vwFactVentas[TiempoID] -> Dim_Tiempo[TiempoID]
- vwFactVentas[ProductID] -> Dim_Producto[ProductID]
- vwFactVentas[CustomerID] -> Dim_Cliente[CustomerID]
- vwFactVentas[EmployeeID] -> Dim_Empleado[EmployeeID]
- vwFactVentas[ShipperID] -> Dim_Shipper[ShipperID]
- vwFactClienteAtendido[TiempoID] -> Dim_Tiempo[TiempoID]
- vwFactClienteAtendido[CustomerID] -> Dim_Cliente[CustomerID]
- vwFactClienteAtendido[EmployeeID] -> Dim_Empleado[EmployeeID]

## 6) Create base measures (DAX)
Create these measures in Power BI (Modeling > New measure):

```
Total Ventas = SUM('vwFactVentas'[SaleAmount])
Unidades = SUM('vwFactVentas'[Quantity])
Ordenes = DISTINCTCOUNT('vwFactVentas'[OrderID])
Ticket Promedio = DIVIDE([Total Ventas], [Ordenes])
Descuento Promedio = AVERAGE('vwFactVentas'[Discount])
Tiempo Respuesta Promedio = AVERAGE('vwFactClienteAtendido'[TiempoRespuesta])
Tiempo Resolucion Promedio = AVERAGE('vwFactClienteAtendido'[TiempoResolucion])
Monto Transaccion Total = SUM('vwFactClienteAtendido'[MontoTransaccion])
```

## 7) Dashboards base (2-3 paginas)
### Dashboard 1: Ventas generales
- Cards: Total Ventas, Unidades, Ordenes, Ticket Promedio
- Line chart: Total Ventas by Dim_Tiempo[Año] and Dim_Tiempo[Mes]
- Bar chart: Total Ventas by Dim_Producto[CategoryName]
- Slicer: Dim_Cliente[Country]

### Dashboard 2: Productos y clientes
- Bar chart: Total Ventas by Dim_Producto[ProductName]
- Bar chart: Total Ventas by Dim_Cliente[CompanyName]
- Matrix: CategoryName x ProductName with Total Ventas
- Slicer: Dim_Empleado[LastName]

### Dashboard 3: Servicio al cliente
- Cards: Tiempo Respuesta Promedio, Tiempo Resolucion Promedio, Monto Transaccion Total
- Line chart: Tiempo Respuesta Promedio by Dim_Tiempo[Año]/[Mes]
- Bar chart: Monto Transaccion Total by Dim_Empleado[LastName]
- Slicer: Dim_Cliente[Country]

## 8) Refresh
- Home > Refresh to validate the model.
- If you run the ETL again, refresh the dataset.
