using GestaoComercial.Web.Data;

var builder = WebApplication.CreateBuilder(args);

// Registra os serviços utilizados pela aplicação.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<SqlConnectionFactory>();
builder.Services.AddScoped<ProdutoRepository>();

var app = builder.Build();

// Configura o tratamento de erros fora do ambiente de desenvolvimento.
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