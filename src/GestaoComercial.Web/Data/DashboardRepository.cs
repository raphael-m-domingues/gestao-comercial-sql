using System.Data;
using Dapper;
using GestaoComercial.Web.Models;

namespace GestaoComercial.Web.Data;

public sealed class DashboardRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public DashboardRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<DashboardViewModel> ObterResumoAsync()
    {
        await using var connection = _connectionFactory.CreateConnection();

        var resumo = await connection
            .QuerySingleAsync<DashboardViewModel>(
                "usp_ResumoEstoque",
                commandType: CommandType.StoredProcedure);

        return resumo;
    }
}