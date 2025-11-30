using BankSim.API.Requests;
using BankSim.Controllers;
using BankSim.Database;
using BankSim.Models.Contas;
using BankSim.Models;
using Microsoft.AspNetCore.Mvc;

namespace BankSim.API.Routes
{
    public static class ContaRoute
    {
        private static ContasController contaController = new();
        public static void MapContaRoutes(this WebApplication app)
        {

            app.MapGet("/contas", ([FromServices] DAL<Conta> dal) => { return contaController.ListarContasController(dal); });
            app.MapPost("/contas", ([FromServices] DAL <Conta> dal, [FromServices] DAL<Cliente> clienteDal, [FromBody] ContaRequest contaRequest) => { return contaController.CriarContaController(dal, clienteDal, contaRequest); });

        }
    }
}
