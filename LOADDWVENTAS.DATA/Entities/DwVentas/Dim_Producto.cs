using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoadDWVentas.Data.Entities.DwVentas
{
    [Table("Dim_Producto")]
    public class DimProducto
    {
        [Key]
        public int ProductID { get; set; }  // Clave primaria

        [Required, MaxLength(40)]
        public required string ProductName { get; set; }

        [Required]
        public int CategoryID { get; set; }

        [Required, MaxLength(15)]
        public required string CategoryName { get; set; }

        [Required]
        public required decimal UnitPrice { get; set; }
    }
}
