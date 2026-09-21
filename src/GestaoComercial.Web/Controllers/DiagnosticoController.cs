using Dapper;
using GestaoComercial.Web.Data;
using Microsoft.AspNetCore.Mvc;

namespace GestaoComercial.Web.Controllers;

[Route("diagnostico")]
public sealed class DiagnosticoController : Controller
{
    private readonly SqlConnectionFactory _connectionFactory;

    public DiagnosticoController(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    [HttpGet("banco")]
    public async Task<IActionResult> Banco()
    {
        const string sql = """
            SELECT
                CAST(SERVERPROPERTY('ServerName') AS NVARCHAR(128))
                    AS NomeServidor,
                DB_NAME() AS BancoAtual,
                COUNT(*) AS TotalTabelas
            FROM sys.tables;
            """;

        await using var connection = _connectionFactory.CreateConnection();

        var resultado =
            await connection.QuerySingleAsync<DiagnosticoBancoResultado>(sql);

        return Ok(new
        {
            Status = "Conexão realizada com sucesso.",
            resultado.NomeServidor,
            resultado.BancoAtual,
            resultado.TotalTabelas
        });
    }
}

public sealed class DiagnosticoBancoResultado
{
    public string NomeServidor { get; set; } = string.Empty;

    public string BancoAtual { get; set; } = string.Empty;

    public int TotalTabelas { get; set; }
}