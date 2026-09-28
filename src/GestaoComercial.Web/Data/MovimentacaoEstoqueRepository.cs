using System.Data;
using Dapper;
using GestaoComercial.Web.Models;

namespace GestaoComercial.Web.Data;

public sealed class MovimentacaoEstoqueRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public MovimentacaoEstoqueRepository(
        SqlConnectionFactory connectionFactory
    )
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<MovimentacaoEstoqueViewModel>>
        ListarAsync(
            DateTime? dataInicial,
            DateTime? dataFinal,
            int? produtoID,
            int? usuarioID,
            string? tipo,
            int limite
        )
    {
        var parametros = new DynamicParameters();

        parametros.Add(
            "@DataInicial",
            dataInicial?.Date,
            DbType.Date
        );

        parametros.Add(
            "@DataFinal",
            dataFinal?.Date,
            DbType.Date
        );

        parametros.Add(
            "@ProdutoID",
            produtoID,
            DbType.Int32
        );

        parametros.Add(
            "@UsuarioID",
            usuarioID,
            DbType.Int32
        );

        parametros.Add(
            "@Tipo",
            string.IsNullOrWhiteSpace(tipo)
                ? null
                : tipo.Trim().ToUpperInvariant(),
            DbType.AnsiString,
            size: 20
        );

        parametros.Add(
            "@Limite",
            limite,
            DbType.Int32
        );

        await using var connection =
            _connectionFactory.CreateConnection();

        return await connection
            .QueryAsync<MovimentacaoEstoqueViewModel>(
                "dbo.usp_ListarMovimentacoesEstoque",
                parametros,
                commandType: CommandType.StoredProcedure
            );
    }

    public async Task<IEnumerable<OpcaoSelecaoViewModel>>
        ListarProdutosAsync()
    {
        const string sql = """
            SELECT
                ProdutoID AS ID,
                CASE
                    WHEN Ativo = 1
                        THEN Nome
                    ELSE CONCAT(Nome, N' (inativo)')
                END AS Nome
            FROM dbo.Produtos
            ORDER BY
                Nome;
            """;

        await using var connection =
            _connectionFactory.CreateConnection();

        return await connection
            .QueryAsync<OpcaoSelecaoViewModel>(sql);
    }

    public async Task<IEnumerable<OpcaoSelecaoViewModel>>
        ListarUsuariosAsync()
    {
        const string sql = """
            SELECT
                UsuarioID AS ID,
                CASE
                    WHEN Ativo = 1
                        THEN Nome
                    ELSE CONCAT(Nome, N' (inativo)')
                END AS Nome
            FROM dbo.Usuarios
            ORDER BY
                Nome;
            """;

        await using var connection =
            _connectionFactory.CreateConnection();

        return await connection
            .QueryAsync<OpcaoSelecaoViewModel>(sql);
    }
}