using System;
using System.Collections.Generic;
using System.Text;

namespace LojaVirtual.Models
{
    internal class Boleto : Pagamento
    {
        public string CodigoBarras { get; set; } = "";

        public DateTime Vencimento { get; set; }
            = DateTime.Today.AddDays(3);

        public override void Processar()
        {
            Processado = true;
        }
    }
}
