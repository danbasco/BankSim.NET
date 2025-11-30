using BankSim.API.Requests;
using BankSim.Database;
using BankSim.Models;
using BankSim.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankSim.Controllers
{
    internal class ClienteController
    {
        private readonly DAL<Cliente> _dal;
        private readonly ClienteService clienteService;

        public ClienteController([FromServices] DAL<Cliente> dal)
        {
            _dal = dal;
            clienteService = new ClienteService(_dal);
        }

        public IResult ListarClientesController()
        {
            return clienteService.ListarTodosClientes();
        }


        public IResult CriarClienteController([FromBody] ClienteRequest clienteRequest)
        {
            return clienteService.CriarCliente(clienteRequest);
        }

        public IResult ObterClientePorIdController(int id)
        {
            return clienteService.ObterClientePorId(id);
        }

        public IResult ListarContasDoClienteController(int id)
        {
            return clienteService.ListarContasDoCliente(id);

        }
    }
}
