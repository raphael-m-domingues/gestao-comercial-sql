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
}