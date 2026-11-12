using System;
using System.Collections.Generic;
using System.Text;

namespace LojaVirtual.Models
{
    internal class Pix : Pagamento
    {
        public string IdentificadorTransacao { get; set; } = "";

        public override void Processar()
        {
            Processado = true;
        }
    }
}
