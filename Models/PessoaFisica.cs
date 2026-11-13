using System;
using System.Collections.Generic;
using System.Text;

namespace LojaVirtual.Models
{
    public class PessoaFisica : Cliente
    {
        public string Cpf { get; set; } = "";
    }
}
