using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using LoadDWVentas.Data.Entities.Northwind;

namespace LoadDWVentas.Data.Context
{
    public class Northwind : DbContext
    {
        private readonly IConfiguration _configuration;

        public Northwind(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DbSet<Categories> Categories { get; set; }
        public DbSet<Customers> Customers { get; set; }
        public DbSet<Employees> Employees { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<Shippers> Shippers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("NorthwindConnection"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderDetails>()
                .HasKey(od => new { od.OrderID, od.ProductID }); // Clave compuesta para OrderDetails
        }
    }
}
