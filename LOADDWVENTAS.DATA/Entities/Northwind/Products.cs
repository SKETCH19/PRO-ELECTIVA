using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoadDWVentas.Data.Entities.Northwind
{
    [Table("Products")]
    public class Products
    {
        [Key]
        public int ProductID { get; set; }

        [Required, MaxLength(40)]
        public required string ProductName { get; set; }

        public int? SupplierID { get; set; }
        public int? CategoryID { get; set; }

        public decimal? UnitPrice { get; set; }

        // Cambiados a 'short?' para coincidir con 'smallint' en SQL Server
        public short? UnitsInStock { get; set; }
        public short? UnitsOnOrder { get; set; }
        public short? ReorderLevel { get; set; }

        public bool Discontinued { get; set; } // 'bit' en SQL Server es 'bool' en C#

        [ForeignKey("CategoryID")]
        public Categories? Category { get; set; }
    }
}
