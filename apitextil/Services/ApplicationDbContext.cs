
using apitextil.Models;

using apitextilECommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace apitextil.Data
{
    internal class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetalleVentas { get; set; }
        public DbSet<MetodoPago> MetodoPagos { get; set; }




        internal async Task SaveChangesAsync()
        {
          
        }
    }
}