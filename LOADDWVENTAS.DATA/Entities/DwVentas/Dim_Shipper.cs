using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoadDWVentas.Data.Entities.DwVentas
{
    [Table("Dim_Shipper")]
    public class DimShipper
    {
        [Key]
        public int ShipperID { get; set; }  // Clave primaria

        [Required, MaxLength(40)]
        public required string CompanyName { get; set; }

        [MaxLength(24)]
        public required string Phone { get; set; }
    }
}
