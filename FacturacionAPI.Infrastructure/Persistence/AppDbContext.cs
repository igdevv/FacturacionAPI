using FacturacionAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FacturacionAPI.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Producto> Productos => Set<Producto>();
        public DbSet<Cliente> Clientes => Set<Cliente>();
        public DbSet<Factura> Facturas => Set<Factura>();
        public DbSet<DetalleFactura> DetallesFactura => Set<DetalleFactura>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Producto>()
                .Property(p => p.Precio)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Producto>().Property(p => p.Stock).IsConcurrencyToken();
            modelBuilder.Entity<Producto>().Property(p => p.Precio).IsConcurrencyToken();
            modelBuilder.Entity<Factura>().Property(f => f.Estado).IsConcurrencyToken();
            // SQL Server almacena datetime2 sin zona; el contrato de la API usa UTC.
            modelBuilder.Entity<Factura>().Property(f => f.FechaEmision)
                .HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            modelBuilder.Entity<Factura>()
                .HasOne(f => f.Cliente).WithMany().HasForeignKey(f => f.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<DetalleFactura>()
                .HasOne(d => d.Producto).WithMany().HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cliente>()
                .HasIndex(c => c.RncCedula)
                .IsUnique();

            modelBuilder.Entity<Factura>()
                .HasMany(f => f.Detalles)
                .WithOne()
                .HasForeignKey(d => d.FacturaId);

            modelBuilder.Entity<DetalleFactura>()
                .Property(d => d.PrecioUnitario)
                .HasPrecision(18, 2);

            modelBuilder.Entity<DetalleFactura>()
                .Property(d => d.TasaItbis)
                .HasPrecision(5, 4);

            modelBuilder.Entity<DetalleFactura>()
                .Ignore(d => d.Subtotal);

            modelBuilder.Entity<DetalleFactura>()
                .Ignore(d => d.Itbis);

            modelBuilder.Entity<DetalleFactura>()
                .Ignore(d => d.Total);

            modelBuilder.Entity<Factura>()
                .Ignore(f => f.Subtotal);

            modelBuilder.Entity<Factura>()
                .Ignore(f => f.Itbis);

            modelBuilder.Entity<Factura>()
                .Ignore(f => f.Total);

            modelBuilder.Entity<Factura>()
                .Ignore(f => f.CantidadTotalProductos);

            modelBuilder.Entity<Factura>()
                .Navigation(f => f.Detalles)
                .HasField("_detalles")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
