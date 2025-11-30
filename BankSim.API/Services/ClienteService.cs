using BankSim.API.Requests;
using BankSim.Database;
using BankSim.Models;
using BankSim.Models.Contas;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace BankSim.Services
{
    internal class ClienteService
    {

        private readonly DAL<Cliente> _dal;

        public ClienteService(DAL<Cliente> dal)
        {
            _dal = dal;
        }

        /**
         * @param dateString Formato "AAAA-MM-DD"
         * returns DateTime
         * Funciona como um conversor simples de string para DateTime
         */
        private DateTime ConvertStringToDateTime(string dateString)
        {
            int[] values = dateString.Split('-').Select(int.Parse).ToArray();
            return new DateTime(values[0], values[1], values[2]);
        }

        /**
         * Valida um número de CPF
         * @param cpf String contendo o número do CPF a ser validado
         * returns bool True se o CPF for válido, false caso contrário
         */
        private bool ValidarCpf(string cpf)
        {
            // Remover caracteres não numéricos
            cpf = new string(cpf.Where(char.IsDigit).ToArray());
            // Verificar se o CPF tem 11 dígitos
            if (cpf.Length != 11)
                return false;
            // Verificar se todos os dígitos são iguais
            if (new string(cpf[0], cpf.Length) == cpf)
                return false;
            // Calcular o primeiro dígito verificador
            int sum = 0;
            for (int i = 0; i < 9; i++)
                sum += (cpf[i] - '0') * (10 - i);
            int firstCheckDigit = sum % 11;
            firstCheckDigit = firstCheckDigit < 2 ? 0 : 11 - firstCheckDigit;
            // Calcular o segundo dígito verificador
            sum = 0;
            for (int i = 0; i < 10; i++)
                sum += (cpf[i] - '0') * (11 - i);
            int secondCheckDigit = sum % 11;
            secondCheckDigit = secondCheckDigit < 2 ? 0 : 11 - secondCheckDigit;
            // Verificar se os dígitos calculados correspondem aos dígitos fornecidos
            return firstCheckDigit == (cpf[9] - '0') && secondCheckDigit == (cpf[10] - '0');
        }

        /**
         * Verifica se um CPF já existe no banco de dados
         * @param cpf String contendo o número do CPF a ser verificado
         * returns bool True se o CPF já existir, false caso contrário
         */
        private bool CPFJaExiste(string cpf)
        {
            var existingCliente = _dal.GetBy(c => c.Cpf == cpf);
            return existingCliente != null;
        }

        /**
         * Cria um novo cliente no banco de dados
         * @param clienteRequest Objeto contendo os dados do cliente a ser criado
         * returns IResult Resultado da operação
         */
        public IResult CriarCliente([FromBody] ClienteRequest clienteRequest)
        {

            if(!ValidarCpf(clienteRequest.CPF)) { return Results.BadRequest("CPF inválido."); }
            if(CPFJaExiste(clienteRequest.CPF)) { return Results.Conflict("CPF já cadastrado."); }

            var cliente = new Cliente(
                clienteRequest.Nome,
                clienteRequest.CPF,
                ConvertStringToDateTime(clienteRequest.DataNascimento)
            );

            _dal.Add(cliente);
            return Results.Created($"/clientes/{cliente.Id}", cliente);
        }

        /**
         * Lista todos os clientes do banco de dados
         * returns IResult Resultado da operação contendo a lista de clientes
         */
        public IResult ListarTodosClientes()
        {
            var clientes = _dal.Query()
                .AsNoTracking()
                .Select(c => new
                {
                    Id = c.Id,
                    Nome = c.Nome,
                    Cpf = c.Cpf,
                    DataNascimento = c.DataNascimento, 
                })
                .ToList(); ;
            return Results.Ok(clientes);
        }

        /**
         * Obtém um cliente pelo seu ID
         * @param id Inteiro representando o ID do cliente a ser obtido
         * returns IResult Resultado da operação contendo o cliente encontrado
         */
        public IResult ObterClientePorId(int id)
        {
            var cliente = _dal.GetBy(c => c.Id == id);
            if (cliente == null)
            {
                return Results.NotFound("Cliente não encontrado.");
            }
            // Mapear para um objeto simples (sem proxy) antes de serializar
            

            var result = new
            {
                cliente.Id,
                cliente.Nome,
                cliente.Cpf,
                cliente.DataNascimento,
            };

            return Results.Ok(result);
        }

        /**
         * Lista todas as contas associadas a um cliente pelo seu ID
         * @param id Inteiro representando o ID do cliente cujas contas serão listadas
         * returns IResult Resultado da operação contendo a lista de contas do cliente
         */
        public IResult ListarContasDoCliente(int id)
        {
            var cliente = _dal.GetBy(c => c.Id == id);
            if (cliente == null)
            {
                return Results.NotFound("Cliente não encontrado.");
            }
            var contas = cliente.Contas?.Select(c => new
            {
                Numero = c.Numero,
                Tipo = c switch
                {
                    ContaCorrente => "Conta Corrente",
                    ContaPoupanca => "Conta Poupança",
                    _ => "Tipo Desconhecido"

                },
                Saldo = c.Saldo
            }) ?? Enumerable.Empty<object>();

            return Results.Ok(contas);
        }

    }
}
