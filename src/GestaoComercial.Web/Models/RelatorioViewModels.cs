using System.ComponentModel.DataAnnotations;

namespace GestaoComercial.Web.Models;

public sealed class RelatorioGerencialViewModel
{
    [Display(Name = "Data inicial")]
    [DataType(DataType.Date)]
    public DateTime DataInicial { get; set; } =
        DateTime.Today.AddDays(-30);

    [Display(Name = "Data final")]
    [DataType(DataType.Date)]
    public DateTime DataFinal { get; set; } =
        DateTime.Today;

    [Display(Name = "Quantidade de produtos")]
    [Range(
        1,
        100,
        ErrorMessage = "O limite deve estar entre 1 e 100."
    )]
    public int Limite { get; set; } = 10;

    public ResumoEstoqueViewModel ResumoEstoque { get; set; } =
        new();

    public IEnumerable<VendaPorPeriodoViewModel> VendasPorPeriodo
        { get; set; } =
        Enumerable.Empty<VendaPorPeriodoViewModel>();

    public IEnumerable<ProdutoMaisVendidoViewModel> ProdutosMaisVendidos
        { get; set; } =
        Enumerable.Empty<ProdutoMaisVendidoViewModel>();
}

public sealed class ResumoEstoqueViewModel
{
    public int TotalProdutosAtivos { get; set; }

    public int TotalUnidadesEstoque { get; set; }

    public decimal ValorCustoEstoque { get; set; }

    public decimal ValorVendaEstoque { get; set; }

    public int ProdutosSemEstoque { get; set; }

    public int ProdutosEstoqueBaixo { get; set; }
}

public sealed class VendaPorPeriodoViewModel
{
    public DateTime Data { get; set; }

    public int QuantidadeVendas { get; set; }

    public decimal TotalVendido { get; set; }

    public decimal TicketMedio { get; set; }
}

public sealed class ProdutoMaisVendidoViewModel
{
    public int ProdutoID { get; set; }

    public string Produto { get; set; } = string.Empty;

    public string Categoria { get; set; } = string.Empty;

    public int QuantidadeVendida { get; set; }

    public decimal TotalVendido { get; set; }
}