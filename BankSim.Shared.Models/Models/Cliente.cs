using BankSim.Models.Contas;
using Microsoft.EntityFrameworkCore;

namespace BankSim.Models;

[Index(nameof(Cpf), IsUnique = true)]
public class Cliente
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Cpf { get; set; }
    public DateTime DataNascimento { get; set; }

    public virtual ICollection<Conta>? Contas { get; set; } = new List<Conta>();

    public Cliente(string nome, string cpf, DateTime dataNascimento)
    {
        Nome = nome;
        Cpf = cpf;
        DataNascimento = dataNascimento;
    }

    public void AdicionarConta(Conta conta)
    {
        Contas?.Add(conta);
    }

}

