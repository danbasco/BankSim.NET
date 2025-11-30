using BankSim.API.Requests;
using BankSim.Controllers;
using Microsoft.AspNetCore.Mvc;
using BankSim.Database;
using BankSim.Models;

namespace BankSim.API.Routes
{
    public static class ClienteRoute
    {
        private static ClienteController clienteController = new();
        public static void MapClienteRoutes(this WebApplication app)
        {

            app.MapGet("/clientes", ([FromServices] DAL<Cliente> dal) => { return clienteController.ListarClientesController(dal); });
            app.MapPost("/clientes", ([FromServices] DAL < Cliente > dal, [FromBody] ClienteRequest clienteRequest) => { return clienteController.CriarClienteController(dal, clienteRequest); });
            app.MapGet("/clientes/{id}", ([FromServices] DAL < Cliente > dal, int id) => { return clienteController.ObterClientePorIdController(dal, id); });
            app.MapGet("/clientes/{id}/contas", ([FromServices] DAL < Cliente > dal, int id) => { return clienteController.ListarContasDoClienteController(dal, id); });

        }
    }
}
