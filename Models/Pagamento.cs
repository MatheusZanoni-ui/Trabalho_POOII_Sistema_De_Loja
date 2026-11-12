using System;
using System.Collections.Generic;
using System.Text;

namespace LojaVirtual.Models
{
    public interface IPagamento
    {
        void Processar();
    }

    public abstract class Pagamento
    {
        public int Id { get; set; }

        public int PedidoId { get; set; }

        public Pedido Pedido { get; set; } = null!;

        public decimal Valor { get; set; }

        public bool Processado { get; set; }

        public abstract void Processar();
    }
}
