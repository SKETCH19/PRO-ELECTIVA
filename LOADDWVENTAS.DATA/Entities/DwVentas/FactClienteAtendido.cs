using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoadDWVentas.Data.Entities.DwVentas
{
    public class Fact_ClienteAtendido
    {
        [Key]
        public int ClienteAtendidoID { get; set; }
        public int OrderID { get; set; }
        public required string CustomerID { get; set; }
        public int EmployeeID { get; set; }
        public int TiempoID { get; set; }
        public int TiempoRespuesta { get; set; }
        public int TiempoResolucion { get; set; }
        public int NumeroInteracciones { get; set; }
        public required string EstadoFinal { get; set; }
        public bool RequirioEscalamiento { get; set; }
        public decimal MontoTransaccion { get; set; }

        // Propiedades de navegación, marcadas como [NotMapped]
        [NotMapped]
        public DimCliente? Cliente { get; set; }

        [NotMapped]
        public DimEmpleado? Empleado { get; set; }

        [NotMapped]
        public DimTiempo? Tiempo { get; set; }
    }
}
