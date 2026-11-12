using System;
using System.Collections.Generic;
using System.Text;

namespace LojaVirtual.Models
{
    public ab class Item
    {
        public int Id { get; set; }

        public string Nome { get; set; } = "";

        public decimal Preco { get; set; }

        public ICollection<Categoria> Categorias { get; set; }
            = new List<Categoria>();

        public ICollection<ItemPedido> ItensPedido { get; set; }
            = new List<ItemPedido>();
    }
}
