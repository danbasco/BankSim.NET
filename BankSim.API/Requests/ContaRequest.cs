using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BankSim.API.Requests
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TipoConta
    {
        Corrente,
        Poupanca
    }
    public record ContaRequest([Required] int Numero, int ClientId);
}
