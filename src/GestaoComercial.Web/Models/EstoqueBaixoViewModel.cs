namespace GestaoComercial.Web.Models;

public sealed class EstoqueBaixoViewModel
{
    public int ProdutoID { get; set; }

    public string Produto { get; set; } = string.Empty;

    public string Categoria { get; set; } = string.Empty;

    public int QuantidadeAtual { get; set; }

    public int EstoqueMinimo { get; set; }

    public string SituacaoEstoque { get; set; } = string.Empty;

    public int QuantidadeParaReposicao { get; set; }

    public DateTime AtualizadoEm { get; set; }
}