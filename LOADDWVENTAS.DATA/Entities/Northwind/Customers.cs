using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoadDWVentas.Data.Entities.Northwind
{
    [Table("Customers")]
    public class Customers
    {
        [Key, MaxLength(5)]
        public required string CustomerID { get; set; }

        [Required, MaxLength(40)]
        public required string CompanyName { get; set; }

        [MaxLength(30)]
        public string? ContactName { get; set; }  // Opcional

        [MaxLength(30)]
        public string? ContactTitle { get; set; } // Opcional

        [MaxLength(60)]
        public string? Address { get; set; }      // Opcional

        [MaxLength(15)]
        public string? City { get; set; }         // Opcional

        [MaxLength(15)]
        public string? Region { get; set; }       // Opcional

        [MaxLength(10)]
        public string? PostalCode { get; set; }   // Opcional

        [MaxLength(15)]
        public required string Country { get; set; }

        [MaxLength(24)]
        public string? Phone { get; set; }        // Opcional

        [MaxLength(24)]
        public string? Fax { get; set; }          // Opcional
    }
}
