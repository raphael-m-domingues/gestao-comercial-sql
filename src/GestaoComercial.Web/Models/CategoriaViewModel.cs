using System.ComponentModel.DataAnnotations;

namespace GestaoComercial.Web.Models;

public sealed class CategoriaViewModel
{
    public int CategoriaID { get; set; }

    [Required(ErrorMessage = "Informe o nome da categoria.")]
    [StringLength(
        60,
        ErrorMessage = "O nome deve possuir no máximo 60 caracteres.")]
    [Display(Name = "Nome")]
    public string Nome { get; set; } = string.Empty;

    [StringLength(
        150,
        ErrorMessage = "A descrição deve possuir no máximo 150 caracteres.")]
    [Display(Name = "Descrição")]
    public string? Descricao { get; set; }

    [Display(Name = "Categoria ativa")]
    public bool Ativo { get; set; } = true;
}