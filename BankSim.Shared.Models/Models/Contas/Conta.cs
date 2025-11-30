using Microsoft.EntityFrameworkCore;

namespace BankSim.Models.Contas
{
    [Index(nameof(Numero), IsUnique = true)]
    public abstract class Conta
    {
        public int Numero { get; set; }
        public float Saldo { get; set; }
        public virtual Cliente? Cliente { get; set; }
        public int ClienteId { get; set; }
        public bool Status { get; set; }
        public int Id { get; set; }

        public virtual ICollection<Transacao>? TransacoesEnviadas { get; set; } = new List<Transacao>();
        public virtual ICollection<Transacao>? TransacoesRecebidas { get; set; } = new List<Transacao>();

        public Conta(int numero, int clienteId)
        {
            Numero = numero;
            ClienteId = clienteId;
            Saldo = 0;
            Status = true;
        }

    }
}
