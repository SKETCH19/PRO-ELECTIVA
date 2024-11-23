using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoadDWVentas.Data.Entities.Northwind
{
    [Table("Employees")]
    public class Employees
    {
        [Key]
        public int EmployeeID { get; set; }

        [Required, MaxLength(20)]
        public required string LastName { get; set; }

        [Required, MaxLength(10)]
        public required string FirstName { get; set; }

        [MaxLength(30)]
        public string? Title { get; set; }         // Opcional

        public DateTime? BirthDate { get; set; }   // Opcional
        public DateTime? HireDate { get; set; }    // Opcional

        [MaxLength(60)]
        public string? Address { get; set; }       // Opcional

        [MaxLength(15)]
        public string? City { get; set; }          // Opcional

        [MaxLength(15)]
        public string? Region { get; set; }        // Opcional

        [MaxLength(10)]
        public string? PostalCode { get; set; }    // Opcional

        [MaxLength(15)]
        public required string Country { get; set; }

        [MaxLength(24)]
        public string? HomePhone { get; set; }     // Opcional

        [MaxLength(4)]
        public string? Extension { get; set; }     // Opcional
    }
}
