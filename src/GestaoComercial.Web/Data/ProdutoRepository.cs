using Dapper;
using GestaoComercial.Web.Models;

namespace GestaoComercial.Web.Data;

public sealed class ProdutoRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public ProdutoRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<ProdutoListagemViewModel>> ListarAsync()
    {
        const string sql = """
            SELECT
                p.ProdutoID,
                p.Nome,
                c.Nome AS Categoria,
                p.PrecoCusto,
                p.PrecoVenda,
                p.EstoqueMinimo,
                e.QuantidadeAtual,
                p.Ativo
            FROM Produtos AS p
            INNER JOIN Categorias AS c
                ON c.CategoriaID = p.CategoriaID
            INNER JOIN Estoques AS e
                ON e.ProdutoID = p.ProdutoID
            ORDER BY p.Nome;
            """;

        await using var connection = _connectionFactory.CreateConnection();

        return await connection
            .QueryAsync<ProdutoListagemViewModel>(sql);
    }

    public async Task<ProdutoFormularioViewModel?> ObterPorIdAsync(
        int produtoID
    )
    {
        const string sql = """
            SELECT
                ProdutoID,
                CategoriaID,
                CodigoBarras,
                Nome,
                Descricao,
                PrecoCusto,
                PrecoVenda,
                EstoqueMinimo,
                Ativo
            FROM Produtos
            WHERE ProdutoID = @ProdutoID;
            """;

        await using var connection = _connectionFactory.CreateConnection();

        return await connection
            .QuerySingleOrDefaultAsync<ProdutoFormularioViewModel>(
                sql,
                new { ProdutoID = produtoID }
            );
    }

    public async Task<IEnumerable<CategoriaViewModel>>
        ListarCategoriasAtivasAsync()
    {
        const string sql = """
            SELECT
                CategoriaID,
                Nome,
                Descricao,
                Ativo
            FROM Categorias
            WHERE Ativo = 1
            ORDER BY Nome;
            """;

        await using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryAsync<CategoriaViewModel>(sql);
    }

    public async Task<int> InserirAsync(
        ProdutoFormularioViewModel produto
    )
    {
        const string sqlProduto = """
            INSERT INTO Produtos
            (
                CategoriaID,
                CodigoBarras,
                Nome,
                Descricao,
                PrecoCusto,
                PrecoVenda,
                EstoqueMinimo,
                Ativo,
                CriadoEm
            )
            OUTPUT INSERTED.ProdutoID
            VALUES
            (
                @CategoriaID,
                NULLIF(LTRIM(RTRIM(@CodigoBarras)), ''),
                LTRIM(RTRIM(@Nome)),
                NULLIF(LTRIM(RTRIM(@Descricao)), ''),
                @PrecoCusto,
                @PrecoVenda,
                @EstoqueMinimo,
                1,
                SYSUTCDATETIME()
            );
            """;

        const string sqlEstoque = """
            INSERT INTO Estoques
            (
                ProdutoID,
                QuantidadeAtual,
                AtualizadoEm
            )
            VALUES
            (
                @ProdutoID,
                0,
                SYSUTCDATETIME()
            );
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        await using var transaction =
            await connection.BeginTransactionAsync();

        try
        {
            var produtoID = await connection.ExecuteScalarAsync<int>(
                sqlProduto,
                produto,
                transaction
            );

            await connection.ExecuteAsync(
                sqlEstoque,
                new { ProdutoID = produtoID },
                transaction
            );

            await transaction.CommitAsync();

            return produtoID;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> AtualizarAsync(
        ProdutoFormularioViewModel produto
    )
    {
        const string sql = """
            UPDATE Produtos
            SET
                CategoriaID = @CategoriaID,
                CodigoBarras =
                    NULLIF(LTRIM(RTRIM(@CodigoBarras)), ''),
                Nome = LTRIM(RTRIM(@Nome)),
                Descricao =
                    NULLIF(LTRIM(RTRIM(@Descricao)), ''),
                PrecoCusto = @PrecoCusto,
                PrecoVenda = @PrecoVenda,
                EstoqueMinimo = @EstoqueMinimo
            WHERE ProdutoID = @ProdutoID;
            """;

        await using var connection = _connectionFactory.CreateConnection();

        var quantidadeAlterada = await connection.ExecuteAsync(
            sql,
            produto
        );

        return quantidadeAlterada > 0;
    }

    public async Task<bool> AlterarStatusAsync(
        int produtoID,
        bool ativo
    )
    {
        const string sql = """
            UPDATE Produtos
            SET Ativo = @Ativo
            WHERE ProdutoID = @ProdutoID;
            """;

        await using var connection = _connectionFactory.CreateConnection();

        var quantidadeAlterada = await connection.ExecuteAsync(
            sql,
            new
            {
                ProdutoID = produtoID,
                Ativo = ativo
            }
        );

        return quantidadeAlterada > 0;
    }
}