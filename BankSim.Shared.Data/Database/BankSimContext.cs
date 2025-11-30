using BankSim.Models;
using BankSim.Models.Contas;
using Microsoft.EntityFrameworkCore;

namespace BankSim.Database
{
    public class BankSimContext : DbContext
    {

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Conta> Contas { get; set; }
        public DbSet<Transacao> Transacoes { get; set; }


        private readonly string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BankSim;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False;Command Timeout=30";

        public BankSimContext(DbContextOptions options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            if (optionsBuilder.IsConfigured) { return; }
            optionsBuilder.UseSqlServer(connectionString)
                .UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mapeamento das transações
            modelBuilder.Entity<Transacao>(b =>
            {
                b.HasKey(t => t.Id);

                // Persistir enum como string em vez de inteiro
                b.Property(t => t.Tipo)
                 .HasConversion<string>()
                 .HasMaxLength(50)
                 .IsRequired();

                b.HasOne(t => t.ContaOrigem)
                 .WithMany(c => c.TransacoesEnviadas)
                 .HasForeignKey(t => t.ContaOrigemId)
                 .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(t => t.ContaDestino)
                 .WithMany(c => c.TransacoesRecebidas)
                 .HasForeignKey(t => t.ContaDestinoId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // Registrar os tipos concretos
            modelBuilder.Entity<ContaCorrente>();
            modelBuilder.Entity<ContaPoupanca>();

            modelBuilder.Entity<Conta>()
                .HasDiscriminator<string>("TipoConta")
                .HasValue<ContaCorrente>("Corrente")
                .HasValue<ContaPoupanca>("Poupanca");

            
        }

    }
}
