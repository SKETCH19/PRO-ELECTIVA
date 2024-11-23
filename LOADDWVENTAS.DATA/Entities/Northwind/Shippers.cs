using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoadDWVentas.Data.Entities.Northwind
{
    [Table("Shippers")]
    public class Shippers
    {
        [Key]
        public int ShipperID { get; set; }

        [Required, MaxLength(40)]
        public required string CompanyName { get; set; }

        [MaxLength(24)]
        public string? Phone { get; set; } // Cambiado a nullable para permitir NULL
    }
}
