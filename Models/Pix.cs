using System;
using System.Collections.Generic;
using System.Text;

namespace LojaVirtual.Models
{
    public class Pix : Pagamento
    {
        public string IdentificadorTransacao { get; set; } = "";

        public override void Processar()
        {
            Processado = true;
        }
    }
}
