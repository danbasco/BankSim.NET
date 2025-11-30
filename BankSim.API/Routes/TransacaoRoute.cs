namespace BankSim.API.Routes
{
    public static class TransacaoRoute
    {
        public static void MapTransacaoRoutes(this WebApplication app)
        {
            app.MapGet("/transacoes", () => "Listar todas as transações");
            app.MapGet("/transacoes/{id}", (int id) => $"Transacoes por ID");
        }
    }
}
