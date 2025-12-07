using AVritmica.BD.Data.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVritmica.BD.Data
{
    public class Context : DbContext
    {
        public DbSet<Carrito> Carritos { get; set; }
        public DbSet<CarritoProducto> CarritoProductos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<CompraDetalle> CompraDetalles { get; set; }
        public DbSet<Consulta> Consultas { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        //public DbSet<Reporte> Reportes { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<StockMovimiento> StockMovimientos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        public Context(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            var cascadeFKs = modelBuilder.Model.GetEntityTypes()
                                         .SelectMany(t => t.GetForeignKeys())
                                         .Where(fk => !fk.IsOwnership
                                                      && fk.DeleteBehavior == DeleteBehavior.Cascade);
            foreach (var fk in cascadeFKs)
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            }

            base.OnModelCreating(modelBuilder);

            // Configurar precisiones decimales
            modelBuilder.Entity<Producto>()
                .Property(p => p.Precio)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<CarritoProducto>()
                .Property(cp => cp.PrecioUnitario)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Carrito>()
                .Property(c => c.MontoTotal)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Carrito>()
                .Property(c => c.Saldo)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Pago>()
                .Property(p => p.MontoPagado)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Pago>()
                .Property(p => p.Saldo)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<CompraDetalle>()
                .Property(cd => cd.PrecioCompra)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<CompraDetalle>()
                .Property(cd => cd.PrecioVentaActualizado)
                .HasColumnType("decimal(18,2)");

            // Configurar relaciones (ahora con DeleteBehavior.Restrict por defecto)
            modelBuilder.Entity<Usuario>()
                .HasMany(u => u.Carritos)
                .WithOne(c => c.Usuario)
                .HasForeignKey(c => c.UsuarioId);

            modelBuilder.Entity<Usuario>()
                .HasMany(u => u.Consultas)
                .WithOne(c => c.Usuario)
                .HasForeignKey(c => c.UsuarioId);

            modelBuilder.Entity<Categoria>()
                .HasMany(c => c.Productos)
                .WithOne(p => p.Categoria)
                .HasForeignKey(p => p.CategoriaId);

            modelBuilder.Entity<Producto>()
                .HasMany(p => p.CarritoProductos)
                .WithOne(cp => cp.Producto)
                .HasForeignKey(cp => cp.ProductoId);

            modelBuilder.Entity<Producto>()
                .HasMany(p => p.StockMovimientos)
                .WithOne(sm => sm.Producto)
                .HasForeignKey(sm => sm.ProductoId);

            modelBuilder.Entity<Producto>()
                .HasMany(p => p.CompraDetalles)
                .WithOne(cd => cd.Producto)
                .HasForeignKey(cd => cd.ProductoId);

            modelBuilder.Entity<Carrito>()
                .HasMany(c => c.CarritoProductos)
                .WithOne(cp => cp.Carrito)
                .HasForeignKey(cp => cp.CarritoId);

            modelBuilder.Entity<Carrito>()
                .HasMany(c => c.Pagos)
                .WithOne(p => p.Carrito)
                .HasForeignKey(p => p.CarritoId);

            modelBuilder.Entity<Compra>()
                .HasMany(c => c.CompraDetalles)
                .WithOne(cd => cd.Compra)
                .HasForeignKey(cd => cd.CompraId);

            // ... otras configuraciones

            /*modelBuilder.Entity<StockMovimiento>()
                .HasOne(sm => sm.Producto)
                .WithMany()
                .HasForeignKey(sm => sm.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);*/

            modelBuilder.Entity<StockMovimiento>()
                .HasOne(sm => sm.Carrito)
                .WithMany()
                .HasForeignKey(sm => sm.CarritoId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
