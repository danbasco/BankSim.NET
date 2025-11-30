using BankSim.Models.Contas;

namespace BankSim.Models;

public class Transacao
{

    public int Id { get; set; }
    public string? Tipo { get; set; }
    public float Valor { get; set; }
    public DateTime Data_Hora { get; set; }
    public virtual Conta? ContaOrigem { get; set; }
    public int? ContaOrigemId { get; set; }
    public virtual Conta? ContaDestino { get; set; }
    public int? ContaDestinoId { get; set; }

    public Transacao(string? tipo, float valor, int? contaOrigemId, int? contaDestinoId)
    {
        Tipo = tipo;
        Valor = valor;
        Data_Hora = DateTime.Now;
        ContaOrigemId = contaOrigemId;
        ContaDestinoId = contaDestinoId;
    }
}

