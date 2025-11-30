using BankSim.API.Requests;
using BankSim.Controllers;
using Microsoft.AspNetCore.Mvc;
using BankSim.Database;
using BankSim.Models;

namespace BankSim.API.Routes
{
    public static class ClienteRoute
    {
        public static void MapClienteRoutes(this WebApplication app)
        {

            app.MapGet("/clientes", ([FromServices] DAL<Cliente> dal) => 
            { return new ClienteController(dal).ListarClientesController(); });

            app.MapPost("/clientes", ([FromServices] DAL<Cliente> dal, [FromBody] ClienteRequest clienteRequest) => 
            { return new ClienteController(dal).CriarClienteController(clienteRequest); });

            app.MapGet("/clientes/{id}", ([FromServices] DAL<Cliente> dal, int id) => 
            { return new ClienteController(dal).ObterClientePorIdController(id); });

            app.MapGet("/clientes/{id}/contas", ([FromServices] DAL<Cliente> dal, int id) => 
            { return new ClienteController(dal).ListarContasDoClienteController(id); });

        }
    }
}
