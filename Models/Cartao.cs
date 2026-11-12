using System;
using System.Collections.Generic;
using System.Text;

namespace LojaVirtual.Models
{
    public class Cartao : Pagamento
    {
        public string Bandeira { get; set; } = "";

        public string UltimosDigitos { get; set; } = "";

        public override void Processar()
        {
            Processado = true;
        }
    }
}
