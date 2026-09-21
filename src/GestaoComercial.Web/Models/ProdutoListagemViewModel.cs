namespace GestaoComercial.Web.Models;

public sealed class ProdutoListagemViewModel
{
    public int ProdutoID { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Categoria { get; set; } = string.Empty;

    public decimal PrecoCusto { get; set; }

    public decimal PrecoVenda { get; set; }

    public int EstoqueMinimo { get; set; }

    public int QuantidadeAtual { get; set; }

    public bool Ativo { get; set; }
}