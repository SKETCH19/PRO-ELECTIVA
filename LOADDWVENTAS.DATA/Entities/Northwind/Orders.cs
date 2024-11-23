using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoadDWVentas.Data.Entities.Northwind
{
    [Table("Orders")]
    public class Orders
    {
        [Key]
        public int OrderID { get; set; }

        [MaxLength(5)]
        public string? CustomerID { get; set; } // Opcional

        public int? EmployeeID { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? RequiredDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public int? ShipVia { get; set; }
        public decimal? Freight { get; set; }

        [MaxLength(40)]
        public string? ShipName { get; set; }      // Opcional

        [MaxLength(60)]
        public string? ShipAddress { get; set; }   // Opcional

        [MaxLength(15)]
        public string? ShipCity { get; set; }      // Opcional

        [MaxLength(15)]
        public string? ShipRegion { get; set; }    // Opcional

        [MaxLength(10)]
        public string? ShipPostalCode { get; set; } // Opcional

        [MaxLength(15)]
        public string? ShipCountry { get; set; }   // Opcional

        // Propiedades de navegación, sin el modificador required
        [ForeignKey("CustomerID")]
        public Customers? Customer { get; set; }  // Opcional

        [ForeignKey("EmployeeID")]
        public Employees? Employee { get; set; }  // Opcional

        [ForeignKey("ShipVia")]
        public Shippers? Shipper { get; set; }    // Opcional
    }
}
