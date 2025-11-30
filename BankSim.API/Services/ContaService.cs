using BankSim.API.Requests;
using BankSim.Database;
using BankSim.Models;
using BankSim.Models.Contas;
using Microsoft.AspNetCore.Mvc;

namespace BankSim.Services
{
    internal class ContaService
    {

        /** Verifica se o usuário com o ID fornecido existe no banco de dados
         * @param clientId ID do cliente a ser verificado
         * returns bool True se o cliente existir, false caso contrário
         */
        private Cliente ValidarUsuarioExistente(DAL<Cliente> clienteDal, int clientId)
        {
            var cliente = clienteDal.GetBy(c => c.Id == clientId);
            return cliente;
        }

        /**
         * Lista todas as contas do banco de dados
         * returns IResult Resultado da operação contendo a lista de contas
         */
        public IResult ListarTodasContas(DAL<Conta> dal)
        {
            var contas = dal.GetAll();
            return Results.Ok(contas);
        }

        /**
         * Cria uma nova conta no banco de dados
         * @param contaRequest Objeto contendo os dados da conta a ser criada
         * returns IResult Resultado da operação
         */
        public IResult CriarConta(DAL<Conta> dal, DAL<Cliente> clienteDal, [FromBody] ContaRequest contaRequest)
        {

            var cliente = ValidarUsuarioExistente(clienteDal, contaRequest.ClientId);

            // Validar ID do cliente
            if (cliente == null)
            {
                return Results.BadRequest($"Cliente com ID {contaRequest.ClientId} não existe.");
            }

            // Validar Numero da conta
            if(dal.GetBy(c => c.Numero == contaRequest.Numero) != null)
            {
                return Results.BadRequest($"Número de conta {contaRequest.Numero} já está em uso.");
            }
            if(contaRequest.Numero <= 0)
            {
                return Results.BadRequest("Número da conta deve ser maior que zero.");
            }

            // Definir tipo de conta
            Conta conta = contaRequest.TipoConta switch
            {
                TipoConta.Corrente => new ContaCorrente(
                    contaRequest.Numero,
                    contaRequest.ClientId
                    ),
                TipoConta.Poupanca => new ContaPoupanca(
                    contaRequest.Numero,
                    contaRequest.ClientId
                    ),
                _ => throw new ArgumentException("Tipo de conta inválido", nameof(contaRequest.TipoConta))
            };

            dal.Add(conta);

            cliente.AdicionarConta(conta);
            clienteDal.Update(cliente);


            return Results.Created($"/contas/{conta.Id}", conta);
        }
    }
}
