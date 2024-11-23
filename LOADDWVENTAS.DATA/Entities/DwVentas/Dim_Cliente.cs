using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoadDWVentas.Data.Entities.DwVentas
{
    [Table("Dim_Cliente")]
    public class DimCliente
    {
        [Key, MaxLength(5)]
        public required string CustomerID { get; set; }  // Clave primaria

        [Required, MaxLength(40)]
        public required string CompanyName { get; set; }

        [Required, MaxLength(15)]
        public required string Country { get; set; }
    }
}
