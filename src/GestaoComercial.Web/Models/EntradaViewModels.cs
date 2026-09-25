using System.ComponentModel.DataAnnotations;

namespace GestaoComercial.Web.Models;

public sealed class EntradaListagemViewModel
{
    public int EntradaID { get; set; }

    public string Fornecedor { get; set; } = string.Empty;

    public string Usuario { get; set; } = string.Empty;

    public DateTime DataEntrada { get; set; }

    public string Status { get; set; } = string.Empty;

    public decimal ValorTotal { get; set; }

    public int QuantidadeItens { get; set; }
}

public sealed class EntradaCriarViewModel
{
    [Display(Name = "Fornecedor")]
    [Range(
        1,
        int.MaxValue,
        ErrorMessage = "Selecione um fornecedor."
    )]
    public int FornecedorID { get; set; }

    public int UsuarioID { get; set; }
}

public sealed class EntradaDetalhesViewModel
{
    public int EntradaID { get; set; }

    public int FornecedorID { get; set; }

    public string Fornecedor { get; set; } = string.Empty;

    public int UsuarioID { get; set; }

    public string Usuario { get; set; } = string.Empty;

    public DateTime DataEntrada { get; set; }

    public string Status { get; set; } = string.Empty;

    public decimal ValorTotal { get; set; }

    public IEnumerable<ItemEntradaViewModel> Itens { get; set; } =
        Enumerable.Empty<ItemEntradaViewModel>();
}

public sealed class ItemEntradaViewModel
{
    public int ItemEntradaID { get; set; }

    public int ProdutoID { get; set; }

    public string Produto { get; set; } = string.Empty;

    public int Quantidade { get; set; }

    public decimal CustoUnitario { get; set; }

    public decimal Subtotal { get; set; }
}

public sealed class AdicionarItemEntradaViewModel
{
    public int EntradaID { get; set; }

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

    [Display(Name = "Custo unitário")]
    [Range(
        0,
        99999999.99,
        ErrorMessage = "Informe um custo unitário válido."
    )]
    public decimal CustoUnitario { get; set; }
}

public sealed class OpcaoSelecaoViewModel
{
    public int ID { get; set; }

    public string Nome { get; set; } = string.Empty;
}