using LojaVirtual.Models;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace LojaVirtual.Data
{
    internal class LojaContext : DBContext
    {
        public DbSet<Cliente> Clientes => Set<Cliente>();

        public DbSet<Item> Itens => Set<Item>();

        public DbSet<Categoria> Categorias => Set<Categoria>();

        public DbSet<Pedido> Pedidos => Set<Pedido>();

        public DbSet<ItemPedido> ItensPedido => Set<ItemPedido>();

        public DbSet<Pagamento> Pagamentos => Set<Pagamento>();

        protected override void OnConfiguring(
            DbContextOptionsBuilder options)
        {
            options.UseSqlServer(
                @"Server=(localdb)\MSSQLLocalDB;
              Database=LojaVirtualDb;
              Trusted_Connection=True;
              TrustServerCertificate=True");
        }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            // HERANÇA CLIENTE - TPH

            mb.Entity<Cliente>()
                .HasDiscriminator<string>("Tipo")
                .HasValue<PessoaFisica>("PF")
                .HasValue<PessoaJuridica>("PJ");


            // HERANÇA ITEM - TPH

            mb.Entity<Item>()
                .HasDiscriminator<string>("Tipo")
                .HasValue<Produto>("Produto")
                .HasValue<Servico>("Servico");


            // HERANÇA PAGAMENTO - TPH

            mb.Entity<Pagamento>()
                .HasDiscriminator<string>("Tipo")
                .HasValue<Cartao>("Cartao")
                .HasValue<Boleto>("Boleto")
                .HasValue<Pix>("Pix");


            // CLIENTE 1:N PEDIDO

            mb.Entity<Pedido>()
                .HasOne(p => p.Cliente)
                .WithMany(c => c.Pedidos)
                .HasForeignKey(p => p.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);


            // PEDIDO 1:N ITEMPEDIDO

            mb.Entity<ItemPedido>()
                .HasOne(ip => ip.Pedido)
                .WithMany(p => p.Itens)
                .HasForeignKey(ip => ip.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);


            // ITEM 1:N ITEMPEDIDO

            mb.Entity<ItemPedido>()
                .HasOne(ip => ip.Item)
                .WithMany(i => i.ItensPedido)
                .HasForeignKey(ip => ip.ItemId)
                .OnDelete(DeleteBehavior.Restrict);


            // PEDIDO 1:1 PAGAMENTO

            mb.Entity<Pedido>()
                .HasOne(p => p.Pagamento)
                .WithOne(p => p.Pedido)
                .HasForeignKey<Pagamento>(p => p.PedidoId);


            // ITEM N:N CATEGORIA

            mb.Entity<Item>()
                .HasMany(i => i.Categorias)
                .WithMany(c => c.Itens);


            // PRECISÃO DOS VALORES DECIMAIS

            mb.Entity<Item>()
                .Property(i => i.Preco)
                .HasPrecision(10, 2);

            mb.Entity<ItemPedido>()
                .Property(i => i.PrecoUnitario)
                .HasPrecision(10, 2);

            mb.Entity<Pagamento>()
                .Property(p => p.Valor)
                .HasPrecision(10, 2);


            // TOTAL DO PEDIDO É CALCULADO

            mb.Entity<Pedido>()
                .Ignore(p => p.Total);
        }
    }
}
