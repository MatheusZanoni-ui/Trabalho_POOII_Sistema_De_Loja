using System;
using System.Collections.Generic;
using System.Text;

namespace LojaVirtual.Models
{
    public enum StatusPedido
    {
        Aberto, 
        Pago, 
        Cancelado
    }

    public class Pedido
    {
        public int Id { get; set; }

        public DateTime Data { get; set; } = DateTime.Now;

        public StatusPedido Status { get; set; }
            = StatusPedido.Aberto;

        public int ClienteId { get; set; }

        public Cliente Cliente { get; set; } = null!;

        public ICollection<ItemPedido> Itens { get; set; }
            = new List<ItemPedido>();

        public Pagamento? Pagamento { get; set; }

        public decimal Total =>
            Itens.Sum(i => i.Quantidade * i.PrecoUnitario);
    }
}
