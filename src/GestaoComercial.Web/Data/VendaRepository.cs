using System.Data;
using Dapper;
using GestaoComercial.Web.Models;

namespace GestaoComercial.Web.Data;

public sealed class VendaRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public VendaRepository(
        SqlConnectionFactory connectionFactory
    )
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<VendaListagemViewModel>>
        ListarAsync()
    {
        const string sql = """
            SELECT
                v.VendaID,
                u.Nome AS Usuario,
                fp.Nome AS FormaPagamento,
                v.DataVenda,
                v.Status,
                v.ValorTotal,
                COUNT(i.ItemVendaID) AS QuantidadeItens
            FROM dbo.Vendas AS v
            INNER JOIN dbo.Usuarios AS u
                ON u.UsuarioID = v.UsuarioID
            INNER JOIN dbo.FormasPagamento AS fp
                ON fp.FormaPagamentoID = v.FormaPagamentoID
            LEFT JOIN dbo.ItensVenda AS i
                ON i.VendaID = v.VendaID
            GROUP BY
                v.VendaID,
                u.Nome,
                fp.Nome,
                v.DataVenda,
                v.Status,
                v.ValorTotal
            ORDER BY
                v.DataVenda DESC,
                v.VendaID DESC;
            """;

        await using var connection =
            _connectionFactory.CreateConnection();

        return await connection
            .QueryAsync<VendaListagemViewModel>(sql);
    }

    public async Task<VendaDetalhesViewModel?>
        ObterPorIdAsync(int vendaID)
    {
        const string sql = """
            SELECT
                v.VendaID,
                v.UsuarioID,
                u.Nome AS Usuario,
                v.FormaPagamentoID,
                fp.Nome AS FormaPagamento,
                v.DataVenda,
                v.Status,
                v.ValorTotal
            FROM dbo.Vendas AS v
            INNER JOIN dbo.Usuarios AS u
                ON u.UsuarioID = v.UsuarioID
            INNER JOIN dbo.FormasPagamento AS fp
                ON fp.FormaPagamentoID = v.FormaPagamentoID
            WHERE v.VendaID = @VendaID;

            SELECT
                i.ItemVendaID,
                i.ProdutoID,
                p.Nome AS Produto,
                i.Quantidade,
                i.PrecoUnitario,
                i.Subtotal
            FROM dbo.ItensVenda AS i
            INNER JOIN dbo.Produtos AS p
                ON p.ProdutoID = i.ProdutoID
            WHERE i.VendaID = @VendaID
            ORDER BY p.Nome;
            """;

        await using var connection =
            _connectionFactory.CreateConnection();

        using var resultados =
            await connection.QueryMultipleAsync(
                sql,
                new
                {
                    VendaID = vendaID
                }
            );

        var venda =
            await resultados
                .ReadSingleOrDefaultAsync<VendaDetalhesViewModel>();

        if (venda is null)
        {
            return null;
        }

        venda.Itens =
            (await resultados
                .ReadAsync<ItemVendaViewModel>())
            .ToList();

        return venda;
    }

    public async Task<IEnumerable<OpcaoSelecaoViewModel>>
        ListarUsuariosAtivosAsync()
    {
        const string sql = """
            SELECT
                UsuarioID AS ID,
                Nome
            FROM dbo.Usuarios
            WHERE Ativo = 1
            ORDER BY Nome;
            """;

        await using var connection =
            _connectionFactory.CreateConnection();

        return await connection
            .QueryAsync<OpcaoSelecaoViewModel>(sql);
    }

    public async Task<IEnumerable<OpcaoSelecaoViewModel>>
        ListarFormasPagamentoAtivasAsync()
    {
        const string sql = """
            SELECT
                FormaPagamentoID AS ID,
                Nome
            FROM dbo.FormasPagamento
            WHERE Ativo = 1
            ORDER BY Nome;
            """;

        await using var connection =
            _connectionFactory.CreateConnection();

        return await connection
            .QueryAsync<OpcaoSelecaoViewModel>(sql);
    }

    public async Task<IEnumerable<OpcaoSelecaoViewModel>>
        ListarProdutosDisponiveisAsync()
    {
        const string sql = """
            SELECT
                p.ProdutoID AS ID,
                CONCAT(
                    p.Nome,
                    N' — estoque: ',
                    e.QuantidadeAtual
                ) AS Nome
            FROM dbo.Produtos AS p
            INNER JOIN dbo.Estoques AS e
                ON e.ProdutoID = p.ProdutoID
            WHERE p.Ativo = 1
              AND e.QuantidadeAtual > 0
            ORDER BY p.Nome;
            """;

        await using var connection =
            _connectionFactory.CreateConnection();

        return await connection
            .QueryAsync<OpcaoSelecaoViewModel>(sql);
    }

    public async Task<int> CriarAsync(
        VendaCriarViewModel venda
    )
    {
        var parametros = new DynamicParameters();

        parametros.Add(
            "@UsuarioID",
            venda.UsuarioID,
            DbType.Int32
        );

        parametros.Add(
            "@FormaPagamentoID",
            venda.FormaPagamentoID,
            DbType.Int32
        );

        parametros.Add(
            "@VendaID",
            dbType: DbType.Int32,
            direction: ParameterDirection.Output
        );

        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            "dbo.usp_CriarVenda",
            parametros,
            commandType: CommandType.StoredProcedure
        );

        return parametros.Get<int>("@VendaID");
    }

    public async Task AdicionarItemAsync(
        AdicionarItemVendaViewModel item
    )
    {
        var parametros = new DynamicParameters();

        parametros.Add(
            "@VendaID",
            item.VendaID,
            DbType.Int32
        );

        parametros.Add(
            "@ProdutoID",
            item.ProdutoID,
            DbType.Int32
        );

        parametros.Add(
            "@Quantidade",
            item.Quantidade,
            DbType.Int32
        );

        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            "dbo.usp_AdicionarItemVenda",
            parametros,
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task AtualizarItemAsync(
        int itemVendaID,
        int quantidade
    )
    {
        var parametros = new DynamicParameters();

        parametros.Add(
            "@ItemVendaID",
            itemVendaID,
            DbType.Int32
        );

        parametros.Add(
            "@Quantidade",
            quantidade,
            DbType.Int32
        );

        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            "dbo.usp_AtualizarItemVenda",
            parametros,
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<int> RemoverItemAsync(
        int itemVendaID
    )
    {
        var parametros = new DynamicParameters();

        parametros.Add(
            "@ItemVendaID",
            itemVendaID,
            DbType.Int32
        );

        await using var connection =
            _connectionFactory.CreateConnection();

        var vendaID =
            await connection.QuerySingleAsync<int>(
                "dbo.usp_RemoverItemVenda",
                parametros,
                commandType: CommandType.StoredProcedure
            );

        return vendaID;
    }

    public async Task ConcluirAsync(int vendaID)
    {
        var parametros = new DynamicParameters();

        parametros.Add(
            "@VendaID",
            vendaID,
            DbType.Int32
        );

        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            "dbo.usp_ConcluirVenda",
            parametros,
            commandType: CommandType.StoredProcedure
        );
    }
}