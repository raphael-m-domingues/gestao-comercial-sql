using System.ComponentModel.DataAnnotations;

namespace GestaoComercial.Web.Models;

public sealed class MovimentacaoEstoqueViewModel
{
    public long MovimentacaoID { get; set; }

    public DateTime DataMovimentacao { get; set; }

    public int ProdutoID { get; set; }

    public string Produto { get; set; } = string.Empty;

    public int UsuarioID { get; set; }

    public string Usuario { get; set; } = string.Empty;

    public string Tipo { get; set; } = string.Empty;

    public int Quantidade { get; set; }

    public string? OrigemTipo { get; set; }

    public int? OrigemID { get; set; }

    public string? Observacao { get; set; }
}

public sealed class MovimentacaoEstoqueFiltroViewModel
{
    [DataType(DataType.Date)]
    [Display(Name = "Data inicial")]
    public DateTime? DataInicial { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Data final")]
    public DateTime? DataFinal { get; set; }

    [Display(Name = "Produto")]
    public int? ProdutoID { get; set; }

    [Display(Name = "Usuário")]
    public int? UsuarioID { get; set; }

    [Display(Name = "Tipo")]
    public string? Tipo { get; set; }

    [Range(
        1,
        1000,
        ErrorMessage = "O limite deve estar entre 1 e 1000."
    )]
    [Display(Name = "Limite")]
    public int Limite { get; set; } = 200;

    public List<MovimentacaoEstoqueViewModel>
        Movimentacoes { get; set; } = [];
}