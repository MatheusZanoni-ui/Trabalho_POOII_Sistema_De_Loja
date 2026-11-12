using System;
using System.Collections.Generic;
using System.Text;

namespace LojaVirtual.Models
{
    internal class PessoaJuridica : Cliente
    {
        public string Cnpj { get; set; } = "";
    }
}
