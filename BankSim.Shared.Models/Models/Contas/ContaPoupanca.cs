using System;
using System.Collections.Generic;
using System.Text;

namespace BankSim.Models.Contas
{
    public class ContaPoupanca : Conta
    {
        public ContaPoupanca(int numero, int clienteId) : base(numero, clienteId)
        {
        }
    }
}
