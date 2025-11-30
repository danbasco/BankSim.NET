namespace BankSim.API.Routes
{
    public static class BankSimRoutes
    {

        public static void MapBankRoutes(this WebApplication app)
        {
            app.MapClienteRoutes();
            app.MapContaRoutes();
            app.MapTransacaoRoutes();
        }


    }
}
