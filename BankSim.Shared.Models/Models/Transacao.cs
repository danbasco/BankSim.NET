using BankSim.Models.Contas;
using System.Text.Json.Serialization;

namespace BankSim.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TipoTransacao
{
    Pix,
    Boleto,
    Cartao,
}


public class Transacao
{

    public int Id { get; set; }
    public TipoTransacao Tipo { get; set; }
    public float Valor { get; set; }
    public DateTime Data_Hora { get; set; }
    public virtual Conta? ContaOrigem { get; set; }
    public int? ContaOrigemId { get; set; }
    public virtual Conta? ContaDestino { get; set; }
    public int? ContaDestinoId { get; set; }

    public Transacao(TipoTransacao tipo, float valor, int? contaOrigemId, int? contaDestinoId)
    {
        Tipo = tipo;
        Valor = valor;
        Data_Hora = DateTime.Now;
        ContaOrigemId = contaOrigemId;
        ContaDestinoId = contaDestinoId;
    }
}

