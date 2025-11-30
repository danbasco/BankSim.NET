using BankSim.API.Routes;
using BankSim.Database;
using BankSim.Models;
using BankSim.Models.Contas;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BankSimContext>((options) =>
{
    options
           .UseSqlServer(builder.Configuration["ConnectionStrings:BankSim"])
           .UseLazyLoadingProxies();
});
builder.Services.AddTransient<DAL<Cliente>>();
builder.Services.AddTransient<DAL<Conta>>();
builder.Services.AddTransient<DAL<Transacao>>();
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options => options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

app.MapBankRoutes();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
