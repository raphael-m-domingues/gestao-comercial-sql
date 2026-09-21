namespace GestaoComercial.Web.Models;

public sealed class DashboardViewModel
{
    public int TotalProdutosAtivos { get; set; }

    public int TotalUnidadesEstoque { get; set; }

    public decimal ValorCustoEstoque { get; set; }

    public decimal ValorVendaEstoque { get; set; }

    public int ProdutosSemEstoque { get; set; }

    public int ProdutosEstoqueBaixo { get; set; }
}