USE DWHNorthwindOrders;
GO
SET NOCOUNT ON;
GO

-- Base views used by the app
IF OBJECT_ID('dbo.vwFactVentas', 'V') IS NOT NULL
    DROP VIEW dbo.vwFactVentas;
GO
CREATE VIEW dbo.vwFactVentas AS
SELECT
    VentaID,
    OrderID,
    TiempoID,
    ProductID,
    CustomerID,
    EmployeeID,
    ShipperID,
    Quantity,
    UnitPrice,
    Discount,
    SaleAmount
FROM dbo.Fact_Ventas;
GO

IF OBJECT_ID('dbo.vwFactClienteAtendido', 'V') IS NOT NULL
    DROP VIEW dbo.vwFactClienteAtendido;
GO
CREATE VIEW dbo.vwFactClienteAtendido AS
SELECT
    ClienteAtendidoID,
    OrderID,
    CustomerID,
    EmployeeID,
    TiempoID,
    TiempoRespuesta,
    TiempoResolucion,
    NumeroInteracciones,
    EstadoFinal,
    RequirioEscalamiento,
    MontoTransaccion
FROM dbo.Fact_ClienteAtendido;
GO

-- Optional analytic views for Power BI dashboards
IF OBJECT_ID('dbo.vwVentasPorMes', 'V') IS NOT NULL
    DROP VIEW dbo.vwVentasPorMes;
GO
CREATE VIEW dbo.vwVentasPorMes AS
SELECT
    t.[Año] AS Anio,
    t.Mes,
    SUM(f.SaleAmount) AS TotalVentas,
    SUM(f.Quantity) AS Unidades,
    COUNT(DISTINCT f.OrderID) AS Ordenes
FROM dbo.Fact_Ventas f
JOIN dbo.Dim_Tiempo t ON t.TiempoID = f.TiempoID
GROUP BY t.[Año], t.Mes;
GO

IF OBJECT_ID('dbo.vwVentasPorCategoria', 'V') IS NOT NULL
    DROP VIEW dbo.vwVentasPorCategoria;
GO
CREATE VIEW dbo.vwVentasPorCategoria AS
SELECT
    p.CategoryName,
    SUM(f.SaleAmount) AS TotalVentas,
    SUM(f.Quantity) AS Unidades
FROM dbo.Fact_Ventas f
JOIN dbo.Dim_Producto p ON p.ProductID = f.ProductID
GROUP BY p.CategoryName;
GO

IF OBJECT_ID('dbo.vwServicioClientePorMes', 'V') IS NOT NULL
    DROP VIEW dbo.vwServicioClientePorMes;
GO
CREATE VIEW dbo.vwServicioClientePorMes AS
SELECT
    t.[Año] AS Anio,
    t.Mes,
    AVG(CAST(f.TiempoRespuesta AS decimal(18,2))) AS TiempoRespuestaPromedio,
    AVG(CAST(f.TiempoResolucion AS decimal(18,2))) AS TiempoResolucionPromedio,
    SUM(f.MontoTransaccion) AS MontoTotal
FROM dbo.Fact_ClienteAtendido f
JOIN dbo.Dim_Tiempo t ON t.TiempoID = f.TiempoID
GROUP BY t.[Año], t.Mes;
GO

-- Indexes to improve query performance
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'UX_Fact_Ventas_Order_Product'
      AND object_id = OBJECT_ID('dbo.Fact_Ventas')
)
BEGIN
    -- Ensure duplicates are removed before creating this unique index.
    CREATE UNIQUE INDEX UX_Fact_Ventas_Order_Product
        ON dbo.Fact_Ventas (OrderID, ProductID);
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'UX_Fact_ClienteAtendido_Order'
      AND object_id = OBJECT_ID('dbo.Fact_ClienteAtendido')
)
BEGIN
    -- Ensure duplicates are removed before creating this unique index.
    CREATE UNIQUE INDEX UX_Fact_ClienteAtendido_Order
        ON dbo.Fact_ClienteAtendido (OrderID);
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_Fact_Ventas_TiempoID'
      AND object_id = OBJECT_ID('dbo.Fact_Ventas')
)
BEGIN
    CREATE INDEX IX_Fact_Ventas_TiempoID ON dbo.Fact_Ventas (TiempoID);
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_Fact_Ventas_ProductID'
      AND object_id = OBJECT_ID('dbo.Fact_Ventas')
)
BEGIN
    CREATE INDEX IX_Fact_Ventas_ProductID ON dbo.Fact_Ventas (ProductID);
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_Fact_Ventas_CustomerID'
      AND object_id = OBJECT_ID('dbo.Fact_Ventas')
)
BEGIN
    CREATE INDEX IX_Fact_Ventas_CustomerID ON dbo.Fact_Ventas (CustomerID);
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_Fact_ClienteAtendido_TiempoID'
      AND object_id = OBJECT_ID('dbo.Fact_ClienteAtendido')
)
BEGIN
    CREATE INDEX IX_Fact_ClienteAtendido_TiempoID ON dbo.Fact_ClienteAtendido (TiempoID);
END
GO
