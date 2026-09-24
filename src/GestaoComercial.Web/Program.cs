using System.Globalization;
using GestaoComercial.Web.Data;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);

// Define a cultura brasileira para números,
// valores monetários e datas.
var culturaBrasileira = new CultureInfo("pt-BR");

CultureInfo.DefaultThreadCurrentCulture =
    culturaBrasileira;

CultureInfo.DefaultThreadCurrentUICulture =
    culturaBrasileira;

// Registra os serviços utilizados pela aplicação.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<SqlConnectionFactory>();

builder.Services.AddScoped<ProdutoRepository>();
builder.Services.AddScoped<DashboardRepository>();
builder.Services.AddScoped<EstoqueRepository>();
builder.Services.AddScoped<CategoriaRepository>();
builder.Services.AddScoped<EntradaRepository>();
builder.Services.AddScoped<VendaRepository>();

var app = builder.Build();

var opcoesLocalizacao = new RequestLocalizationOptions
{
    DefaultRequestCulture =
        new RequestCulture(culturaBrasileira),

    SupportedCultures = new[]
    {
        culturaBrasileira
    },

    SupportedUICultures = new[]
    {
        culturaBrasileira
    }
};

app.UseRequestLocalization(opcoesLocalizacao);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();