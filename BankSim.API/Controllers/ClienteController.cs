using BankSim.API.Requests;
using BankSim.Database;
using BankSim.Models;
using BankSim.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankSim.Controllers
{
    internal class ClienteController
    {

        private ClienteService clienteService = new ClienteService( );


        public IResult ListarClientesController(DAL<Cliente> dal)
        {
            return clienteService.ListarTodosClientes(dal);
        }


        public IResult CriarClienteController(DAL<Cliente> dal, [FromBody] ClienteRequest clienteRequest)
        {
            return clienteService.CriarCliente(dal, clienteRequest);
        }

        public IResult ObterClientePorIdController(DAL<Cliente> dal, int id)
        {
            return clienteService.ObterClientePorId(dal, id);
        }

        public IResult ListarContasDoClienteController(DAL<Cliente> dal, int id)
        {
            return clienteService.ListarContasDoCliente(dal, id);

        }
    }
}
