using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadDWVentas.Data.Entities.DwVentas
{
    public class vwFactClienteAtendido
    {
        public int ClienteAtendidoID { get; set; }
        public int OrderID { get; set; }
        public required string CustomerID { get; set; } // Manejo de nchar
        public int EmployeeID { get; set; }
        public int TiempoID { get; set; }
        public int TiempoRespuesta { get; set; }
        public int TiempoResolucion { get; set; }
        public int NumeroInteracciones { get; set; }
        public required string EstadoFinal { get; set; } // varchar(20)
        public bool RequirioEscalamiento { get; set; } // bit
        public decimal MontoTransaccion { get; set; } // money mapea a decimal
    }
}
