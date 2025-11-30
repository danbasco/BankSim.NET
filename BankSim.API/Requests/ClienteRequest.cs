using System.ComponentModel.DataAnnotations;

namespace BankSim.API.Requests
{
    public record ClienteRequest([Required] String Nome, [Required] String DataNascimento, [Required] String CPF);
}
