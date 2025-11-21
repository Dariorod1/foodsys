using Microsoft.EntityFrameworkCore;
using CafeteriaApi.Models;

namespace CafeteriaApi.Data
{
    public class CafeteriaDbContext : DbContext
    {
        public CafeteriaDbContext(DbContextOptions<CafeteriaDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Mesa> Mesas { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<ItemPedido> ItemsPedidos { get; set; }
        public DbSet<CierreCaja> CierresCaja { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de entidad Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuarios");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Correo).IsRequired().HasMaxLength(256);
                entity.Property(e => e.NombreCompleto).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Teléfono).HasMaxLength(20);
                entity.HasIndex(e => e.Correo).IsUnique();
            });

            // Configuración de entidad Producto
            modelBuilder.Entity<Producto>(entity =>
            {
                entity.ToTable("Productos");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Descripcion).HasMaxLength(500);
                entity.Property(e => e.Precio).HasPrecision(10, 2);
            });

            // Configuración de entidad Mesa
            modelBuilder.Entity<Mesa>(entity =>
            {
                entity.ToTable("Mesas");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Numero).IsRequired();
                entity.Property(e => e.Estado).IsRequired();
                entity.HasIndex(e => e.Numero).IsUnique();
            });

            // Configuración de entidad Pedido
            modelBuilder.Entity<Pedido>(entity =>
            {
                entity.ToTable("Pedidos");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.MontoTotal).HasPrecision(10, 2);
                entity.HasOne(e => e.Usuario)
                    .WithMany(u => u.Pedidos)
                    .HasForeignKey(e => e.UsuarioId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(e => e.Cajero)
                    .WithMany()
                    .HasForeignKey(e => e.CajeroId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Mesa)
                    .WithMany(m => m.Pedidos)
                    .HasForeignKey(e => e.MesaId)
                    .OnDelete(DeleteBehavior.SetNull);
                    
                entity.HasMany(e => e.Articulos)
                    .WithOne(oi => oi.Pedido)
                    .HasForeignKey(oi => oi.PedidoId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración de entidad ItemPedido
            modelBuilder.Entity<ItemPedido>(entity =>
            {
                entity.ToTable("ItemsPedidos");
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Pedido)
                    .WithMany(p => p.Articulos)
                    .HasForeignKey(e => e.PedidoId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Producto).WithMany().HasForeignKey(e => e.ProductoId);
                entity.Property(e => e.PrecioUnitario).HasPrecision(10, 2);
                entity.Property(e => e.Subtotal).HasPrecision(10, 2);
            });

            // Configuración de entidad CierreCaja
            modelBuilder.Entity<CierreCaja>(entity =>
            {
                entity.ToTable("CierresCaja");
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Cajero).WithMany().HasForeignKey(e => e.CajeroId);
                entity.Property(e => e.MontoInicial).HasPrecision(10, 2);
                entity.Property(e => e.MontoEsperado).HasPrecision(10, 2);
                entity.Property(e => e.MontoReal).HasPrecision(10, 2);
                entity.Property(e => e.Diferencia).HasPrecision(10, 2);
            });
        }
    }
}
