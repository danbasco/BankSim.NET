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

        private readonly DAL<Conta> dal;
        private readonly DAL<Cliente> clienteDal;
        private readonly ContaService contaService;

        public ContasController([FromServices] DAL<Conta> dal, [FromServices] DAL<Cliente> clienteDal)
        {

            this.dal = dal;
            this.clienteDal = clienteDal;
            contaService = new ContaService(dal, clienteDal);
        }

        

        public IResult ListarContasController()
        {
            return contaService.ListarTodasContas();
        }


        public IResult CriarContaController([FromBody] ContaRequest contaRequest, TipoConta tipo)
        {
            return contaService.CriarConta(contaRequest, tipo);
        }

        public IResult DepositarController([FromRoute] int id, float valor)
        {
            return contaService.Depositar(id, valor);
        }

        public IResult SacarController([FromRoute] int id, float valor)
        {
            return contaService.Sacar(id, valor);
        }

        public IResult TransferirController([FromRoute]int id, int contaDestinoId, float valor, TipoTransacao tipo)
        {
            return contaService.Transferir(id, contaDestinoId, valor, tipo);
        }

        public IResult ListarTransferenciasController([FromRoute] int id)
        {
            return contaService.ListarTransferencias(id);
        }

    }
}
