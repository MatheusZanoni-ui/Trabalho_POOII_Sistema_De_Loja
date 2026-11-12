using System;
using System.Collections.Generic;
using System.Text;

namespace LojaVirtual.Models
{
    internal class Categoria
    {
        public int Id { get; set; }

        public string Nome { get; set; } = "";

        public ICollection<Item> Itens { get; set; }
            = new List<Item>();
    }
}
