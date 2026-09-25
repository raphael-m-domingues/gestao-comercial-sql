using System.ComponentModel.DataAnnotations;

namespace GestaoComercial.Web.Models;

public sealed class VendaListagemViewModel
{
    public int VendaID { get; set; }

    public string Usuario { get; set; } = string.Empty;

    public string FormaPagamento { get; set; } = string.Empty;

    public DateTime DataVenda { get; set; }

    public string Status { get; set; } = string.Empty;

    public decimal ValorTotal { get; set; }

    public int QuantidadeItens { get; set; }
}

public sealed class VendaCriarViewModel
{
    public int UsuarioID { get; set; }

    [Display(Name = "Forma de pagamento")]
    [Range(
        1,
        int.MaxValue,
        ErrorMessage = "Selecione uma forma de pagamento."
    )]
    public int FormaPagamentoID { get; set; }
}

public sealed class VendaDetalhesViewModel
{
    public int VendaID { get; set; }

    public int UsuarioID { get; set; }

    public string Usuario { get; set; } = string.Empty;

    public int FormaPagamentoID { get; set; }

    public string FormaPagamento { get; set; } = string.Empty;

    public DateTime DataVenda { get; set; }

    public string Status { get; set; } = string.Empty;

    public decimal ValorTotal { get; set; }

    public IEnumerable<ItemVendaViewModel> Itens { get; set; } =
        Enumerable.Empty<ItemVendaViewModel>();
}

public sealed class ItemVendaViewModel
{
    public int ItemVendaID { get; set; }

    public int ProdutoID { get; set; }

    public string Produto { get; set; } = string.Empty;

    public int Quantidade { get; set; }

    public decimal PrecoUnitario { get; set; }

    public decimal Subtotal { get; set; }
}

public sealed class AdicionarItemVendaViewModel
{
    public int VendaID { get; set; }

    [Display(Name = "Produto")]
    [Range(
        1,
        int.MaxValue,
        ErrorMessage = "Selecione um produto."
    )]
    public int ProdutoID { get; set; }

    [Range(
        1,
        int.MaxValue,
        ErrorMessage = "A quantidade deve ser maior que zero."
    )]
    public int Quantidade { get; set; } = 1;
}