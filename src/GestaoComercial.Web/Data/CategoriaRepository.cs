using Dapper;
using GestaoComercial.Web.Models;

namespace GestaoComercial.Web.Data;

public sealed class CategoriaRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public CategoriaRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<CategoriaViewModel>> ListarAsync()
    {
        const string sql = """
            SELECT
                CategoriaID,
                Nome,
                Descricao,
                Ativo
            FROM Categorias
            ORDER BY Nome;
            """;

        await using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryAsync<CategoriaViewModel>(sql);
    }

    public async Task<CategoriaViewModel?> ObterPorIdAsync(
        int categoriaID)
    {
        const string sql = """
            SELECT
                CategoriaID,
                Nome,
                Descricao,
                Ativo
            FROM Categorias
            WHERE CategoriaID = @CategoriaID;
            """;

        await using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<CategoriaViewModel>(
            sql,
            new { CategoriaID = categoriaID });
    }

    public async Task<int> InserirAsync(CategoriaViewModel categoria)
    {
        const string sql = """
            INSERT INTO Categorias
            (
                Nome,
                Descricao,
                Ativo
            )
            OUTPUT INSERTED.CategoriaID
            VALUES
            (
                @Nome,
                @Descricao,
                @Ativo
            );
            """;

        await using var connection = _connectionFactory.CreateConnection();

        return await connection.ExecuteScalarAsync<int>(sql, categoria);
    }

    public async Task<bool> AtualizarAsync(CategoriaViewModel categoria)
    {
        const string sql = """
            UPDATE Categorias
            SET
                Nome = @Nome,
                Descricao = @Descricao,
                Ativo = @Ativo
            WHERE CategoriaID = @CategoriaID;
            """;

        await using var connection = _connectionFactory.CreateConnection();

        var registrosAfetados =
            await connection.ExecuteAsync(sql, categoria);

        return registrosAfetados == 1;
    }

    public async Task<bool> AlterarStatusAsync(
        int categoriaID,
        bool ativo)
    {
        const string sql = """
            UPDATE Categorias
            SET Ativo = @Ativo
            WHERE CategoriaID = @CategoriaID;
            """;

        await using var connection = _connectionFactory.CreateConnection();

        var registrosAfetados = await connection.ExecuteAsync(
            sql,
            new
            {
                CategoriaID = categoriaID,
                Ativo = ativo
            });

        return registrosAfetados == 1;
    }
}