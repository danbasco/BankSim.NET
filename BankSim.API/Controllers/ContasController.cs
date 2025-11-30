using BankSim.API.Requests;
using BankSim.Database;
using BankSim.Models;
using BankSim.Models.Contas;
using BankSim.Services;
using Microsoft.AspNetCore.Mvc;


namespace BankSim.Controllers
{
    internal class ContasController
    {
        public IResult ListarContasController(DAL<Conta> dal)
        {
            return new ContaService().ListarTodasContas(dal);
        }


        public IResult CriarContaController(DAL<Conta> dal, DAL<Cliente> clienteDal, [FromBody] ContaRequest contaRequest)
        {
            return new ContaService().CriarConta(dal, clienteDal, contaRequest);
        }

    }
}
