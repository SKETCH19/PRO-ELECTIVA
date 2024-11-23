using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadDWVentas.Data.Entities.DwVentas
{
    public class vwFactVentas
    {
        public int VentaID { get; set; }
        public int OrderID { get; set; }
        public int TiempoID { get; set; }
        public int ProductID { get; set; }
        public required string CustomerID { get; set; } // Manejo de nchar
        public int EmployeeID { get; set; }
        public int ShipperID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; } // money mapea a decimal
        public double Discount { get; set; } // float mapea a double
        public decimal SaleAmount { get; set; } // money mapea a decimal
    }


}
