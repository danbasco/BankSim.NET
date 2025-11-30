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
        public static void MapContaRoutes(this WebApplication app)
        {

            app.MapGet("/contas", ([FromServices] DAL<Conta> dal, [FromServices] DAL<Cliente> clienteDal) => 
            { return new ContasController(dal, clienteDal).ListarContasController(); });

            app.MapPost("/contas", ([FromServices] DAL <Conta> dal, [FromServices] DAL <Cliente> clienteDal, [FromBody] ContaRequest contaRequest, TipoConta tipo) => 
            { return  new ContasController(dal, clienteDal).CriarContaController(contaRequest, tipo); });

            app.MapPost("/contas/{id}/depositar", ([FromServices] DAL <Conta> dal, [FromServices] DAL <Cliente> clienteDal, [FromRoute] int id, float valor) => 
            { return new ContasController(dal, clienteDal).DepositarController(id, valor); });

            app.MapPost("/contas/{id}/sacar", ([FromServices] DAL <Conta> dal, [FromServices] DAL <Cliente> clienteDal, [FromRoute] int id, float valor) => 
            { return new ContasController(dal, clienteDal).SacarController(id, valor); });

            app.MapPost("/contas/{id}/transferir", ([FromServices] DAL<Conta> dal, [FromServices] DAL<Cliente> clienteDal, [FromRoute] int id, int contaDestinoId, float valor, TipoTransacao tipo) => 
            { return new ContasController(dal, clienteDal).TransferirController(id, contaDestinoId, valor, tipo); });

            app.MapGet("/contas/{id}/transferir", ([FromServices] DAL<Conta> dal, [FromServices] DAL<Cliente> clienteDal, [FromRoute] int id) =>
            { return new ContasController(dal, clienteDal).ListarTransferenciasController(id); });

        }
    }
}
