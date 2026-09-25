using System.Data;
using Dapper;
using GestaoComercial.Web.Models;

namespace GestaoComercial.Web.Data;

public sealed class RelatorioRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public RelatorioRepository(
        SqlConnectionFactory connectionFactory
    )
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<ResumoEstoqueViewModel>
        ObterResumoEstoqueAsync()
    {
        await using var connection =
            _connectionFactory.CreateConnection();

        var resumo = await connection
            .QuerySingleAsync<ResumoEstoqueViewModel>(
                "usp_ResumoEstoque",
                commandType: CommandType.StoredProcedure
            );

        return resumo;
    }

    public async Task<IEnumerable<VendaPorPeriodoViewModel>>
        ListarVendasPorPeriodoAsync(
            DateTime dataInicial,
            DateTime dataFinal
        )
    {
        await using var connection =
            _connectionFactory.CreateConnection();

        var vendas = await connection
            .QueryAsync<VendaPorPeriodoViewModel>(
                "usp_VendasPorPeriodo",
                new
                {
                    DataInicial = dataInicial.Date,
                    DataFinal = dataFinal.Date
                },
                commandType: CommandType.StoredProcedure
            );

        return vendas;
    }

    public async Task<IEnumerable<ProdutoMaisVendidoViewModel>>
        ListarProdutosMaisVendidosAsync(
            DateTime dataInicial,
            DateTime dataFinal,
            int limite
        )
    {
        await using var connection =
            _connectionFactory.CreateConnection();

        var produtos = await connection
            .QueryAsync<ProdutoMaisVendidoViewModel>(
                "usp_ProdutosMaisVendidos",
                new
                {
                    DataInicial = dataInicial.Date,
                    DataFinal = dataFinal.Date,
                    Limite = limite
                },
                commandType: CommandType.StoredProcedure
            );

        return produtos;
    }

    public async Task<RelatorioGerencialViewModel>
        ObterRelatorioAsync(
            DateTime dataInicial,
            DateTime dataFinal,
            int limite
        )
    {
        var resumo =
            await ObterResumoEstoqueAsync();

        var vendas =
            await ListarVendasPorPeriodoAsync(
                dataInicial,
                dataFinal
            );

        var produtos =
            await ListarProdutosMaisVendidosAsync(
                dataInicial,
                dataFinal,
                limite
            );

        return new RelatorioGerencialViewModel
        {
            DataInicial = dataInicial,
            DataFinal = dataFinal,
            Limite = limite,
            ResumoEstoque = resumo,
            VendasPorPeriodo = vendas,
            ProdutosMaisVendidos = produtos
        };
    }
}