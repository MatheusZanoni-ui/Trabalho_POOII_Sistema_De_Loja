using System;
using System.Collections.Generic;
using System.Text;

namespace LojaVirtual.Models
{
    internal class ItemPedido
    {
        public int Id { get; set; }

        public int PedidoId { get; set; }

        public Pedido Pedido { get; set; } = null!;

        public int ItemId { get; set; }

        public Item Item { get; set; } = null!;

        public int Quantidade { get; set; }

        public decimal PrecoUnitario { get; set; }
    }
}
