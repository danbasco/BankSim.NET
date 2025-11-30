using System;
using System.Collections.Generic;
using System.Text;

namespace BankSim.Models.Contas
{
    public class ContaCorrente : Conta
    {
        public ContaCorrente(int numero, int clienteId) : base(numero, clienteId)
        {
        }
    }
}
