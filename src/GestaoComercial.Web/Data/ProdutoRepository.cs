using Dapper;
using GestaoComercial.Web.Models;

namespace GestaoComercial.Web.Data;

public sealed class ProdutoRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public ProdutoRepository(
        SqlConnectionFactory connectionFactory
    )
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<ProdutoConsultaViewModel> ConsultarAsync(
        ProdutoConsultaViewModel consulta
    )
    {
        const string sql = """
            SELECT
                COUNT(*)
            FROM dbo.Produtos AS p
            INNER JOIN dbo.Categorias AS c
                ON c.CategoriaID = p.CategoriaID
            INNER JOIN dbo.Estoques AS e
                ON e.ProdutoID = p.ProdutoID
            WHERE
                (
                    @Busca IS NULL
                    OR p.Nome LIKE '%' + @Busca + '%'
                    OR p.CodigoBarras LIKE '%' + @Busca + '%'
                )
                AND
                (
                    @CategoriaID IS NULL
                    OR p.CategoriaID = @CategoriaID
                )
                AND
                (
                    @Status = 'TODOS'
                    OR
                    (
                        @Status = 'ATIVOS'
                        AND p.Ativo = 1
                    )
                    OR
                    (
                        @Status = 'INATIVOS'
                        AND p.Ativo = 0
                    )
                )
                AND
                (
                    @SituacaoEstoque = 'TODOS'
                    OR
                    (
                        @SituacaoEstoque = 'SEM_ESTOQUE'
                        AND e.QuantidadeAtual = 0
                    )
                    OR
                    (
                        @SituacaoEstoque = 'ESTOQUE_BAIXO'
                        AND e.QuantidadeAtual > 0
                        AND e.QuantidadeAtual <= p.EstoqueMinimo
                    )
                    OR
                    (
                        @SituacaoEstoque = 'NORMAL'
                        AND e.QuantidadeAtual > p.EstoqueMinimo
                    )
                );

            SELECT
                p.ProdutoID,
                p.Nome,
                c.Nome AS Categoria,
                p.PrecoCusto,
                p.PrecoVenda,
                p.EstoqueMinimo,
                e.QuantidadeAtual,
                p.Ativo
            FROM dbo.Produtos AS p
            INNER JOIN dbo.Categorias AS c
                ON c.CategoriaID = p.CategoriaID
            INNER JOIN dbo.Estoques AS e
                ON e.ProdutoID = p.ProdutoID
            WHERE
                (
                    @Busca IS NULL
                    OR p.Nome LIKE '%' + @Busca + '%'
                    OR p.CodigoBarras LIKE '%' + @Busca + '%'
                )
                AND
                (
                    @CategoriaID IS NULL
                    OR p.CategoriaID = @CategoriaID
                )
                AND
                (
                    @Status = 'TODOS'
                    OR
                    (
                        @Status = 'ATIVOS'
                        AND p.Ativo = 1
                    )
                    OR
                    (
                        @Status = 'INATIVOS'
                        AND p.Ativo = 0
                    )
                )
                AND
                (
                    @SituacaoEstoque = 'TODOS'
                    OR
                    (
                        @SituacaoEstoque = 'SEM_ESTOQUE'
                        AND e.QuantidadeAtual = 0
                    )
                    OR
                    (
                        @SituacaoEstoque = 'ESTOQUE_BAIXO'
                        AND e.QuantidadeAtual > 0
                        AND e.QuantidadeAtual <= p.EstoqueMinimo
                    )
                    OR
                    (
                        @SituacaoEstoque = 'NORMAL'
                        AND e.QuantidadeAtual > p.EstoqueMinimo
                    )
                )
            ORDER BY
                p.Nome,
                p.ProdutoID
            OFFSET @Deslocamento ROWS
            FETCH NEXT @TamanhoPagina ROWS ONLY;
            """;

        var busca = string.IsNullOrWhiteSpace(consulta.Busca)
            ? null
            : consulta.Busca.Trim();

        var parametros = new
        {
            Busca = busca,
            consulta.CategoriaID,
            consulta.Status,
            consulta.SituacaoEstoque,
            Deslocamento =
                (consulta.Pagina - 1) * consulta.TamanhoPagina,
            consulta.TamanhoPagina
        };

        await using var connection =
            _connectionFactory.CreateConnection();

        using var resultados =
            await connection.QueryMultipleAsync(
                sql,
                parametros
            );

        consulta.TotalRegistros =
            await resultados.ReadSingleAsync<int>();

        consulta.Produtos =
            (await resultados
                .ReadAsync<ProdutoListagemViewModel>())
            .ToList();

        return consulta;
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
            FROM dbo.Produtos
            WHERE ProdutoID = @ProdutoID;
            """;

        await using var connection =
            _connectionFactory.CreateConnection();

        return await connection
            .QuerySingleOrDefaultAsync<ProdutoFormularioViewModel>(
                sql,
                new
                {
                    ProdutoID = produtoID
                }
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
            FROM dbo.Categorias
            WHERE Ativo = 1
            ORDER BY Nome;
            """;

        await using var connection =
            _connectionFactory.CreateConnection();

        return await connection
            .QueryAsync<CategoriaViewModel>(sql);
    }

    public async Task<int> InserirAsync(
        ProdutoFormularioViewModel produto
    )
    {
        const string sqlProduto = """
            INSERT INTO dbo.Produtos
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
            INSERT INTO dbo.Estoques
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

        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.OpenAsync();

        await using var transaction =
            await connection.BeginTransactionAsync();

        try
        {
            var produtoID =
                await connection.ExecuteScalarAsync<int>(
                    sqlProduto,
                    produto,
                    transaction
                );

            await connection.ExecuteAsync(
                sqlEstoque,
                new
                {
                    ProdutoID = produtoID
                },
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
            UPDATE dbo.Produtos
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

        await using var connection =
            _connectionFactory.CreateConnection();

        var quantidadeAlterada =
            await connection.ExecuteAsync(
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
            UPDATE dbo.Produtos
            SET Ativo = @Ativo
            WHERE ProdutoID = @ProdutoID;
            """;

        await using var connection =
            _connectionFactory.CreateConnection();

        var quantidadeAlterada =
            await connection.ExecuteAsync(
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