using System.Collections.Generic;
using System.Threading.Tasks;
using LoadDWVentas.Data.Entities.DwVentas;

namespace LoadDWVentas.Data.Interfaces
{
    public interface IDataServiceswhventas
    {
        Task LoadClientesAsync();
        Task LoadProductosAsync();
        Task LoadEmpleadosAsync();
        Task LoadShippersAsync();
        Task LoadVentasAsync();
        Task LoadTiempoAsync();
        Task LoadClienteAtendidoAsync();
        Task LimpiarTablasAsync();

        // Métodos para las vistas
        Task<List<vwFactVentas>> GetFactVentasAsync();
        Task<List<vwFactClienteAtendido>> GetFactClienteAtendidoAsync();
    }
}
