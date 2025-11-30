using BankSim.API.Requests;
using BankSim.Database;
using BankSim.Models;
using BankSim.Models.Contas;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankSim.Services
{
    internal class ContaService
    {


        private DAL<Conta> dal;
        private DAL<Cliente> clienteDal;

        public ContaService(DAL<Conta> dal, DAL<Cliente> clienteDal)
        {
            this.dal = dal;
            this.clienteDal = clienteDal;
        }

        /** Verifica se o usuário com o ID fornecido existe no banco de dados
         * @param clientId ID do cliente a ser verificado
         * returns bool True se o cliente existir, false caso contrário
         */
        private Cliente ValidarUsuarioExistente(int clientId)
        {
            var cliente = clienteDal.GetBy(c => c.Id == clientId);
            return cliente;
        }

        /**
         * Lista todas as contas do banco de dados
         * returns IResult Resultado da operação contendo a lista de contas
         */
        public IResult ListarTodasContas()
        {
            var contas = dal.Query()
                .Include(c => c.Cliente)
                .AsNoTracking()
                .Select(c => new
                {
                    id = c.Id,
                    numero = c.Numero,
                    saldo = c.Saldo,
                    tipo = EF.Property<string>(c, "TipoConta"),
                    cliente = c.Cliente == null ? null : new
                    {
                        id = c.Cliente.Id,
                        nome = c.Cliente.Nome,
                        cpf = c.Cliente.Cpf,
                        dataNascimento = c.Cliente.DataNascimento
                    },
                    status = c.Status,
                })
                .ToList();
            return Results.Ok(contas);
        }

        /**
         * Cria uma nova conta no banco de dados
         * @param contaRequest Objeto contendo os dados da conta a ser criada
         * returns IResult Resultado da operação
         */
        public IResult CriarConta([FromBody] ContaRequest contaRequest, TipoConta tipo)
        {

            var cliente = ValidarUsuarioExistente(contaRequest.ClientId);

            // Validar ID do cliente
            if (cliente == null)
            {
                return Results.BadRequest($"Cliente com ID {contaRequest.ClientId} não existe.");
            }

            // Validar Numero da conta
            if (dal.GetBy(c => c.Numero == contaRequest.Numero) != null)
            {
                return Results.BadRequest($"Número de conta {contaRequest.Numero} já está em uso.");
            }
            if (contaRequest.Numero <= 0)
            {
                return Results.BadRequest("Número da conta deve ser maior que zero.");
            }

            // Definir tipo de conta
            Conta conta = tipo switch
            {
                TipoConta.Corrente => new ContaCorrente(
                    contaRequest.Numero,
                    contaRequest.ClientId
                    ),
                TipoConta.Poupanca => new ContaPoupanca(
                    contaRequest.Numero,
                    contaRequest.ClientId
                    ),
                _ => throw new ArgumentException("Tipo de conta inválido", nameof(tipo))
            };

            cliente.AdicionarConta(conta);
            clienteDal.Update(cliente);


            return Results.Created($"/contas/{conta.Id}", conta);
        }

        /**
         * Realiza um depósito em uma conta existente
         * @param id ID da conta onde o depósito será realizado
         * @param valor Valor a ser depositado
         * returns IResult Resultado da operação
         */
        public IResult Depositar(int id, float valor)
        {

            var conta = dal.GetBy(c => c.Id == id);
            if (conta == null) { return Results.NotFound($"Conta com ID {id} não encontrada."); }

            // Validar valor do depósito
            if (valor <= 0) { return Results.BadRequest("O valor do depósito deve ser maior que zero."); }

            // Conta desativada
            if (!conta.Status)
            {
                return Results.BadRequest("Não é possível realizar depósitos em contas desativadas.");
            }

            // Realizar depósito
            conta.Depositar(valor);

            // Criando a transação de depósito
            Transacao transacao = new(
                TipoTransacao.Boleto, valor, null, conta.Id
                );

            conta.AdicionarTransacaoRecebida(transacao);

            var accountDto = new
            {
                Id = conta.Id,
                Saldo = conta.Saldo,
            };

            var transacaoDto = new
            {
                Id = transacao.Id,
                Tipo = transacao.Tipo.ToString(),
                Valor = transacao.Valor,
            };


            dal.Update(conta);

            return Results.Ok(new { Account = accountDto, Transaction = transacaoDto });
        }

        public IResult Sacar(int id, float valor)
        {

            var conta = dal.GetBy(c => c.Id == id);
            if (conta == null) { return Results.NotFound($"Conta com ID {id} não encontrada."); }

            // Conta desativada
            if (!conta.Status)
            { return Results.BadRequest("Não é possível realizar saques em contas desativadas."); }

            // Validar valor do saque
            if (valor <= 0) { return Results.BadRequest("O valor do saque deve ser maior que zero."); }
            if (conta.Saldo < valor) { return Results.BadRequest("Saldo insuficiente para realizar o saque."); }

            conta.Sacar(valor);

            // Criando a transação de Saque
            Transacao transacao = new(
                TipoTransacao.Boleto, valor, conta.Id, null
                );

            conta.AdicionarTransacaoEnviada(transacao);

            dal.Update(conta);

            var accountDto = new
            {
                Id = conta.Id,
                Saldo = conta.Saldo,
            };

            var transacaoDto = new
            {
                Id = transacao.Id,
                Tipo = transacao.Tipo.ToString(),
                Valor = transacao.Valor,
            };

            return Results.Ok(new { Account = accountDto, Transaction = transacaoDto });

        }


        /** Transfere um valor entre duas contas
         * @param id ID da conta de origem
         * @param contaDestinoId ID da conta de destino
         * @param valor Valor a ser transferido
         * returns IResult Resultado da operação
         */
        public IResult Transferir(int id, int contaDestinoId, float valor, TipoTransacao tipo)
        {

            // Validação das contas
            var contaOrigem = dal.GetBy(c => c.Id == id);
            var contaDestino = dal.GetBy(c => c.Id == contaDestinoId);

            if (contaOrigem == null || contaDestino == null)
            {
                return Results.NotFound("Conta de origem ou destino não encontrada.");
            }

            // Contas desativadas
            if (!contaOrigem.Status || !contaDestino.Status)
            {
                return Results.BadRequest("Não é possível realizar transferências com contas desativadas.");
            }

            // Validar valor da transferência
            if (valor <= 0)
            {
                return Results.BadRequest("O valor da transferência deve ser maior que zero.");
            }

            if (contaOrigem.Saldo < valor)
            {
                return Results.BadRequest("Saldo insuficiente para realizar a transferência.");
            }

            // Criando as transações de Transferência
            Transacao transacaoOrigem = new(
                tipo, valor, contaOrigem.Id, contaDestino.Id
                );
            Transacao transacaoDestino = new(
                tipo, valor, contaDestino.Id, contaOrigem.Id
                );

            contaOrigem.AdicionarTransacaoEnviada(transacaoOrigem);
            contaDestino.AdicionarTransacaoRecebida(transacaoDestino);

            // Realizar transferência
            contaOrigem.Sacar(valor);
            contaDestino.Depositar(valor);

            dal.Update(contaOrigem);
            dal.Update(contaDestino);

            var transacaoDTO = new
            {
                Id_Origem = transacaoOrigem.Id,
                Id_Destino = transacaoDestino.Id,
                Valor = valor,
                Tipo = tipo.ToString()
            };

            return Results.Ok(transacaoDTO);

        }

        public IResult ListarTransferencias(int id)
        {

            var conta = dal.GetBy(c => c.Id == id);
            if (conta == null) { return Results.NotFound($"Conta com ID {id} não encontrada."); }

            var TransacoesEnviadasDTO = conta.TransacoesEnviadas?
                .Select(t => new
                {
                    Id = t.Id,
                    Tipo = t.Tipo.ToString(),
                    Valor = t.Valor,
                    ContaDestinoId = t.ContaDestinoId,
                })
                .ToList();
            var TransacoesRecebidasDTO = conta.TransacoesRecebidas?
                .Select(t => new
                {
                    Id = t.Id,
                    Tipo = t.Tipo.ToString(),
                    Valor = t.Valor,
                    ContaOrigemId = t.ContaOrigemId,
                })
                .ToList();

            return Results.Ok(new { TransacoesEnviadas = TransacoesEnviadasDTO, TransacoesRecebidas = TransacoesRecebidasDTO });

        }
    }
}
