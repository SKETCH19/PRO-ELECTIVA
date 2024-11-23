using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using LoadDWVentas.Data.Entities.DwVentas;

namespace LoadDWVentas.Data.Context
{
    public class DWHNorthwindOrders : DbContext
    {
        private readonly IConfiguration _configuration;

        public DWHNorthwindOrders(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Tablas
        public DbSet<DimTiempo> DimTiempo { get; set; }
        public DbSet<DimProducto> DimProducto { get; set; }
        public DbSet<DimCliente> DimCliente { get; set; }
        public DbSet<DimEmpleado> DimEmpleado { get; set; }
        public DbSet<DimShipper> DimShipper { get; set; }
        public DbSet<Fact_Ventas> Fact_Ventas { get; set; }
        public DbSet<Fact_ClienteAtendido> Fact_ClienteAtendido { get; set; }

        // Vistas
        public DbSet<vwFactVentas> FactVentasView { get; set; }
        public DbSet<vwFactClienteAtendido> ClienteAtendidoView { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DWHNorthwindOrdersConnection"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración para Fact_Ventas
            modelBuilder.Entity<Fact_Ventas>().ToTable("Fact_Ventas");

            // Configuración para Fact_ClienteAtendido
            modelBuilder.Entity<Fact_ClienteAtendido>().ToTable("Fact_ClienteAtendido");

            // Configuración para vwFactVentas
            modelBuilder.Entity<vwFactVentas>()
                .ToView("vwFactVentas")
                .HasKey(v => v.VentaID);

            modelBuilder.Entity<vwFactVentas>()
                .Property(v => v.CustomerID)
                .HasMaxLength(5) // Coincide con nchar(5) en SQL Server
                .IsFixedLength(); // Especifica que es un campo de longitud fija

            // Configuración para vwFactClienteAtendido
            modelBuilder.Entity<vwFactClienteAtendido>()
                .ToView("vwFactClienteAtendido")
                .HasKey(c => c.ClienteAtendidoID);

            base.OnModelCreating(modelBuilder);
        }
    }
}
