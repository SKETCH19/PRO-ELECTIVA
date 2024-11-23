using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoadDWVentas.Data.Entities.Northwind
{
    [Table("Categories")]
    public class Categories
    {
        [Key]
        public int CategoryID { get; set; }

        [Required, MaxLength(15)]
        public required string CategoryName { get; set; }

        [MaxLength(255)]
        public string? Description { get; set; } // Opcional para permitir NULL
    }
}
