using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoadDWVentas.Data.Entities.DwVentas
{
    public class Fact_Ventas
    {
        [Key]
        public int VentaID { get; set; }
        public int OrderID { get; set; }
        public int TiempoID { get; set; }
        public int ProductID { get; set; }
        public required string CustomerID { get; set; }
        public int EmployeeID { get; set; }
        public int ShipperID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public float Discount { get; set; }
        public decimal SaleAmount { get; set; }

        // Propiedades de navegación, marcadas como [NotMapped]
        [NotMapped]
        public DimTiempo? Tiempo { get; set; }

        [NotMapped]
        public DimProducto? Producto { get; set; }

        [NotMapped]
        public DimCliente? Cliente { get; set; }

        [NotMapped]
        public DimEmpleado? Empleado { get; set; }

        [NotMapped]
        public DimShipper? Shipper { get; set; }
    }
}
