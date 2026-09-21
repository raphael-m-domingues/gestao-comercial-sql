using System.Data;
using Dapper;
using GestaoComercial.Web.Models;

namespace GestaoComercial.Web.Data;

public sealed class EstoqueRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public EstoqueRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<EstoqueBaixoViewModel>>
        ListarProdutosEstoqueBaixoAsync()
    {
        await using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryAsync<EstoqueBaixoViewModel>(
            "usp_ListarProdutosEstoqueBaixo",
            commandType: CommandType.StoredProcedure);
    }
}