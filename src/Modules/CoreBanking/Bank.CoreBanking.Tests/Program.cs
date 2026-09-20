using Bank.BuildingBlocks.Application.Modules;
using Bank.CoreBanking.Presentation;
using Microsoft.EntityFrameworkCore;
using Bank.CoreBanking.Infrastructure.Data;

var builder = WebApplication.CreateBuilder();

// Add services
builder.Services.AddScoped<IModule, CoreBankingModule>();

// Add database
builder.Services.AddDbContext<CoreBankingDbContext>(
    options => options.UseInMemoryDatabase("CoreBankingTestDb")
);

// Add CoreBanking services
var config = builder.Configuration;
builder.Services.AddCoreBankingInfrastructure(
    config.GetConnectionString("DefaultConnection") ?? "InMemory"
);
builder.Services.AddCoreBankingPresentationServices();

// Add Mvc & API Explorer for endpoints
builder.Services.AddRouting();

var app = builder.Build();

// Map endpoints
app.UseRouting();
app.MapCoreBankingEndpoints();

app.Run();

// Make Program accessible to WebApplicationFactory
public partial class Program { }
