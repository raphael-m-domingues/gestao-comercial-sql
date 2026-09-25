using System.Globalization;
using GestaoComercial.Web.Data;
using GestaoComercial.Web.Models;
using GestaoComercial.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);

// Define a cultura brasileira para números,
// valores monetários e datas.
var culturaBrasileira = new CultureInfo("pt-BR");

CultureInfo.DefaultThreadCurrentCulture =
    culturaBrasileira;

CultureInfo.DefaultThreadCurrentUICulture =
    culturaBrasileira;

// Registra o MVC.
builder.Services.AddControllersWithViews();

// Configura a autenticação por cookie.
builder.Services
    .AddAuthentication(
        CookieAuthenticationDefaults.AuthenticationScheme
    )
    .AddCookie(opcoes =>
    {
        opcoes.LoginPath = "/Autenticacao/Login";
        opcoes.AccessDeniedPath = "/Autenticacao/AcessoNegado";

        opcoes.Cookie.Name =
            "GestaoComercial.Autenticacao";

        opcoes.Cookie.HttpOnly = true;
        opcoes.Cookie.SameSite = SameSiteMode.Lax;
        opcoes.Cookie.SecurePolicy =
            CookieSecurePolicy.SameAsRequest;

        opcoes.ExpireTimeSpan =
            TimeSpan.FromHours(8);

        opcoes.SlidingExpiration = true;

        opcoes.EventsType =
            typeof(AutenticacaoCookieEvents);
    });

// Exige autenticação por padrão em toda a aplicação.
// Somente ações com AllowAnonymous ficam públicas.
builder.Services.AddAuthorization(opcoes =>
{
    opcoes.FallbackPolicy =
        new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
});

// Registra a conexão e os repositórios.
builder.Services.AddSingleton<SqlConnectionFactory>();

builder.Services.AddScoped<ProdutoRepository>();
builder.Services.AddScoped<DashboardRepository>();
builder.Services.AddScoped<EstoqueRepository>();
builder.Services.AddScoped<CategoriaRepository>();
builder.Services.AddScoped<EntradaRepository>();
builder.Services.AddScoped<VendaRepository>();
builder.Services.AddScoped<RelatorioRepository>();
builder.Services.AddScoped<AutenticacaoRepository>();

// Registra os serviços de autenticação.
builder.Services.AddScoped<
    IPasswordHasher<UsuarioAutenticacaoViewModel>,
    PasswordHasher<UsuarioAutenticacaoViewModel>
>();

builder.Services.AddScoped<AutenticacaoCookieEvents>();
builder.Services.AddScoped<AdministradorInicializador>();

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

app.UseAuthentication();
app.UseAuthorization();

// Arquivos CSS, JavaScript e demais recursos visuais
// precisam estar disponíveis na tela pública de login.
app.MapStaticAssets()
    .AllowAnonymous();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Cria o primeiro administrador somente quando
// ainda não existe um administrador ativo.
await using (var scope = app.Services.CreateAsyncScope())
{
    var inicializador =
        scope.ServiceProvider
            .GetRequiredService<AdministradorInicializador>();

    await inicializador.InicializarAsync();
}

app.Run();