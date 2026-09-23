using System.Data;
using Dapper;
using GestaoComercial.Web.Models;

namespace GestaoComercial.Web.Data;

public sealed class EntradaRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public EntradaRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<EntradaListagemViewModel>>
        ListarAsync()
    {
        const string sql = """
            SELECT
                e.EntradaID,
                COALESCE(
                    NULLIF(f.NomeFantasia, ''),
                    f.RazaoSocial
                ) AS Fornecedor,
                u.Nome AS Usuario,
                e.DataEntrada,
                e.Status,
                e.ValorTotal,
                COUNT(ie.ItemEntradaID) AS QuantidadeItens
            FROM Entradas AS e
            INNER JOIN Fornecedores AS f
                ON f.FornecedorID = e.FornecedorID
            INNER JOIN Usuarios AS u
                ON u.UsuarioID = e.UsuarioID
            LEFT JOIN ItensEntrada AS ie
                ON ie.EntradaID = e.EntradaID
            GROUP BY
                e.EntradaID,
                f.NomeFantasia,
                f.RazaoSocial,
                u.Nome,
                e.DataEntrada,
                e.Status,
                e.ValorTotal
            ORDER BY e.EntradaID DESC;
            """;

        await using var connection =
            _connectionFactory.CreateConnection();

        return await connection
            .QueryAsync<EntradaListagemViewModel>(sql);
    }

    public async Task<EntradaDetalhesViewModel?> ObterPorIdAsync(
        int entradaID
    )
    {
        const string sql = """
            SELECT
                e.EntradaID,
                e.FornecedorID,
                COALESCE(
                    NULLIF(f.NomeFantasia, ''),
                    f.RazaoSocial
                ) AS Fornecedor,
                e.UsuarioID,
                u.Nome AS Usuario,
                e.DataEntrada,
                e.Status,
                e.ValorTotal
            FROM Entradas AS e
            INNER JOIN Fornecedores AS f
                ON f.FornecedorID = e.FornecedorID
            INNER JOIN Usuarios AS u
                ON u.UsuarioID = e.UsuarioID
            WHERE e.EntradaID = @EntradaID;

            SELECT
                ie.ItemEntradaID,
                ie.ProdutoID,
                p.Nome AS Produto,
                ie.Quantidade,
                ie.CustoUnitario,
                ie.Subtotal
            FROM ItensEntrada AS ie
            INNER JOIN Produtos AS p
                ON p.ProdutoID = ie.ProdutoID
            WHERE ie.EntradaID = @EntradaID
            ORDER BY p.Nome;
            """;

        await using var connection =
            _connectionFactory.CreateConnection();

        using var resultado = await connection.QueryMultipleAsync(
            sql,
            new { EntradaID = entradaID }
        );

        var entrada = await resultado
            .ReadSingleOrDefaultAsync<EntradaDetalhesViewModel>();

        var itens = await resultado
            .ReadAsync<ItemEntradaViewModel>();

        if (entrada is not null)
        {
            entrada.Itens = itens;
        }

        return entrada;
    }

    public async Task<IEnumerable<OpcaoSelecaoViewModel>>
        ListarFornecedoresAtivosAsync()
    {
        const string sql = """
            SELECT
                FornecedorID AS ID,
                COALESCE(
                    NULLIF(NomeFantasia, ''),
                    RazaoSocial
                ) AS Nome
            FROM Fornecedores
            WHERE Ativo = 1
            ORDER BY Nome;
            """;

        await using var connection =
            _connectionFactory.CreateConnection();

        return await connection
            .QueryAsync<OpcaoSelecaoViewModel>(sql);
    }

    public async Task<IEnumerable<OpcaoSelecaoViewModel>>
        ListarUsuariosAtivosAsync()
    {
        const string sql = """
            SELECT
                UsuarioID AS ID,
                Nome
            FROM Usuarios
            WHERE Ativo = 1
            ORDER BY Nome;
            """;

        await using var connection =
            _connectionFactory.CreateConnection();

        return await connection
            .QueryAsync<OpcaoSelecaoViewModel>(sql);
    }

    public async Task<IEnumerable<OpcaoSelecaoViewModel>>
        ListarProdutosAtivosAsync()
    {
        const string sql = """
            SELECT
                ProdutoID AS ID,
                Nome
            FROM Produtos
            WHERE Ativo = 1
            ORDER BY Nome;
            """;

        await using var connection =
            _connectionFactory.CreateConnection();

        return await connection
            .QueryAsync<OpcaoSelecaoViewModel>(sql);
    }

    public async Task<int> CriarAsync(
        EntradaCriarViewModel entrada
    )
    {
        var parametros = new DynamicParameters();

        parametros.Add(
            "@FornecedorID",
            entrada.FornecedorID,
            DbType.Int32
        );

        parametros.Add(
            "@UsuarioID",
            entrada.UsuarioID,
            DbType.Int32
        );

        parametros.Add(
            "@EntradaID",
            dbType: DbType.Int32,
            direction: ParameterDirection.Output
        );

        await using var connection =
            _connectionFactory.CreateConnection();

        var entradaCriada =
            await connection.QuerySingleAsync<EntradaDetalhesViewModel>(
                "dbo.usp_CriarEntrada",
                parametros,
                commandType: CommandType.StoredProcedure
            );

        return entradaCriada.EntradaID;
    }

    public async Task AdicionarItemAsync(
        AdicionarItemEntradaViewModel item
    )
    {
        var parametros = new
        {
            item.EntradaID,
            item.ProdutoID,
            item.Quantidade,
            item.CustoUnitario
        };

        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.QuerySingleAsync<ItemEntradaViewModel>(
            "dbo.usp_AdicionarItemEntrada",
            parametros,
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task AtualizarItemAsync(
        int itemEntradaID,
        int quantidade,
        decimal custoUnitario
    )
    {
        var parametros = new
        {
            ItemEntradaID = itemEntradaID,
            Quantidade = quantidade,
            CustoUnitario = custoUnitario
        };

        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.QuerySingleAsync<ItemEntradaViewModel>(
            "dbo.usp_AtualizarItemEntrada",
            parametros,
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<int> RemoverItemAsync(int itemEntradaID)
    {
        await using var connection =
            _connectionFactory.CreateConnection();

        return await connection.QuerySingleAsync<int>(
            "dbo.usp_RemoverItemEntrada",
            new { ItemEntradaID = itemEntradaID },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task ConfirmarAsync(int entradaID)
    {
        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.QuerySingleAsync<EntradaDetalhesViewModel>(
            "dbo.usp_ConfirmarEntrada",
            new { EntradaID = entradaID },
            commandType: CommandType.StoredProcedure
        );
    }
}