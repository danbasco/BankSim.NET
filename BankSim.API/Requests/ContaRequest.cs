using System.ComponentModel.DataAnnotations;

namespace BankSim.API.Requests
{
    public enum TipoConta
    {
        Corrente,
        Poupanca
    }
    public record ContaRequest([Required] int Numero, int ClientId, TipoConta TipoConta);
}
