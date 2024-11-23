using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoadDWVentas.Data.Entities.DwVentas
{
    [Table("Dim_Empleado")]
    public class DimEmpleado
    {
        [Key]
        public int EmployeeID { get; set; }  // Clave primaria

        [Required, MaxLength(20)]
        public required string LastName { get; set; }

        [Required, MaxLength(10)]
        public required string FirstName { get; set; }
    }
}
