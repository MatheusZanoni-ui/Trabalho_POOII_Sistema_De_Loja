using System;
using System.Collections.Generic;
using System.Text;

namespace LojaVirtual.Models
{
    public class PessoaJuridica : Cliente
    {
        public string Cnpj { get; set; } = "";
    }
}
