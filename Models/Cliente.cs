using System;
using System.Collections.Generic;
using System.Text;

namespace LojaVirtual.Models
{
    public abstract class Cliente
    {
        public int Id { get; set; }
        public string Nome { get; set; } = "";
        public string Email { get; set; } = "";
        public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    }
}
