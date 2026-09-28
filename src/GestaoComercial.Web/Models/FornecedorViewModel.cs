using System.ComponentModel.DataAnnotations;

namespace GestaoComercial.Web.Models;

public sealed class FornecedorViewModel
{
    public int FornecedorID { get; set; }

    [Required(ErrorMessage = "Informe a razão social.")]
    [StringLength(
        120,
        ErrorMessage =
            "A razão social deve possuir no máximo 120 caracteres."
    )]
    [Display(Name = "Razão social")]
    public string RazaoSocial { get; set; } = string.Empty;

    [StringLength(
        120,
        ErrorMessage =
            "O nome fantasia deve possuir no máximo 120 caracteres."
    )]
    [Display(Name = "Nome fantasia")]
    public string? NomeFantasia { get; set; }

    [RegularExpression(
        @"^\d{2}\.?\d{3}\.?\d{3}/?\d{4}-?\d{2}$",
        ErrorMessage = "Informe um CNPJ válido com 14 dígitos."
    )]
    [Display(Name = "CNPJ")]
    public string? CNPJ { get; set; }

    [StringLength(
        20,
        ErrorMessage =
            "O telefone deve possuir no máximo 20 caracteres."
    )]
    [Display(Name = "Telefone")]
    public string? Telefone { get; set; }

    [EmailAddress(
        ErrorMessage = "Informe um endereço de e-mail válido."
    )]
    [StringLength(
        150,
        ErrorMessage =
            "O e-mail deve possuir no máximo 150 caracteres."
    )]
    [Display(Name = "E-mail")]
    public string? Email { get; set; }

    [Display(Name = "Fornecedor ativo")]
    public bool Ativo { get; set; } = true;

    public string CNPJFormatado
    {
        get
        {
            if (string.IsNullOrWhiteSpace(CNPJ)
                || CNPJ.Length != 14)
            {
                return "Não informado";
            }

            return Convert.ToUInt64(CNPJ)
                .ToString(@"00\.000\.000\/0000\-00");
        }
    }
}