using System;
using System.Collections.Generic;
using System.Text;

namespace LojaVirtual.Models
{
    internal class PessoaFisica : Cliente
    {
        public string Cpf { get; set; } = "";
    }
}
