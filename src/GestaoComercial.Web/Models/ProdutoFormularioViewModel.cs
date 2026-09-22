using System.ComponentModel.DataAnnotations;

namespace GestaoComercial.Web.Models;

public class ProdutoFormularioViewModel
{
    public int ProdutoID { get; set; }

    [Display(Name = "Categoria")]
    [Range(
        1,
        int.MaxValue,
        ErrorMessage = "Selecione uma categoria."
    )]
    public int CategoriaID { get; set; }

    [Display(Name = "Código de barras")]
    [StringLength(
        14,
        ErrorMessage =
            "O código de barras deve possuir no máximo 14 caracteres."
    )]
    public string? CodigoBarras { get; set; }

    [Required(ErrorMessage = "Informe o nome do produto.")]
    [StringLength(
        120,
        ErrorMessage = "O nome deve possuir no máximo 120 caracteres."
    )]
    public string Nome { get; set; } = string.Empty;

    [Display(Name = "Descrição")]
    [StringLength(
        255,
        ErrorMessage = "A descrição deve possuir no máximo 255 caracteres."
    )]
    public string? Descricao { get; set; }

    [Display(Name = "Preço de custo")]
    [Range(
        0,
        99999999.99,
        ErrorMessage = "Informe um preço de custo válido."
    )]
    public decimal PrecoCusto { get; set; }

    [Display(Name = "Preço de venda")]
    [Range(
        0,
        99999999.99,
        ErrorMessage = "Informe um preço de venda válido."
    )]
    public decimal PrecoVenda { get; set; }

    [Display(Name = "Estoque mínimo")]
    [Range(
        0,
        int.MaxValue,
        ErrorMessage = "O estoque mínimo não pode ser negativo."
    )]
    public int EstoqueMinimo { get; set; }

    public bool Ativo { get; set; } = true;
}