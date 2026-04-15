using AneDoces.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AneDoces.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<Orcamento> Orcamentos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Nome)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(c => c.Telefone)
                    .HasMaxLength(20);

                entity.Property(c => c.WhatsApp)
                    .HasMaxLength(20);

                entity.Property(c => c.Endereco)
                    .HasMaxLength(255);

                entity.Property(c => c.Observacao)
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<Pedido>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Descricao)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(p => p.Valor)
                    .HasColumnType("decimal(18,2)");

                entity.Property(p => p.DataPedido)
                    .IsRequired();

                entity.Property(p => p.Status)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.HasOne(p => p.Cliente)
                    .WithMany()
                    .HasForeignKey(p => p.ClienteId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Orcamento>(entity =>
            {
                entity.HasKey(o => o.Id);

                entity.Property(o => o.Descricao)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(o => o.Valor)
                    .HasColumnType("decimal(18,2)");

                entity.Property(o => o.DataOrcamento)
                    .IsRequired();

                entity.Property(o => o.Status)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(o => o.ConvertidoEmPedido)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(o => o.PedidoId)
                    .IsRequired(false);

                entity.HasOne(o => o.Cliente)
                    .WithMany()
                    .HasForeignKey(o => o.ClienteId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}