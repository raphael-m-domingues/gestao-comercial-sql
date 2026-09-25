using System.ComponentModel.DataAnnotations;

namespace GestaoComercial.Web.Models;

public sealed class LoginViewModel
{
    [Required(ErrorMessage = "Informe o e-mail.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a senha.")]
    [DataType(DataType.Password)]
    [Display(Name = "Senha")]
    public string Senha { get; set; } = string.Empty;

    [Display(Name = "Manter conectado")]
    public bool ManterConectado { get; set; }

    public string? ReturnUrl { get; set; }
}

public sealed class UsuarioAutenticacaoViewModel
{
    public int UsuarioID { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string SenhaHash { get; set; } = string.Empty;

    public string Perfil { get; set; } = string.Empty;

    public bool Ativo { get; set; }
}

public sealed class UsuarioListagemViewModel
{
    public int UsuarioID { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Perfil { get; set; } = string.Empty;

    public bool Ativo { get; set; }

    public DateTime CriadoEm { get; set; }
}

public sealed class UsuarioCriarViewModel
{
    [Required(ErrorMessage = "Informe o nome.")]
    [StringLength(
        100,
        ErrorMessage = "O nome deve possuir no máximo 100 caracteres."
    )]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o e-mail.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    [StringLength(
        150,
        ErrorMessage = "O e-mail deve possuir no máximo 150 caracteres."
    )]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;

    [Range(
        1,
        int.MaxValue,
        ErrorMessage = "Selecione um perfil."
    )]
    [Display(Name = "Perfil")]
    public int PerfilID { get; set; }

    [Required(ErrorMessage = "Informe a senha.")]
    [StringLength(
        100,
        MinimumLength = 8,
        ErrorMessage = "A senha deve possuir pelo menos 8 caracteres."
    )]
    [DataType(DataType.Password)]
    [Display(Name = "Senha")]
    public string Senha { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirme a senha.")]
    [DataType(DataType.Password)]
    [Compare(
        nameof(Senha),
        ErrorMessage = "A confirmação da senha não confere."
    )]
    [Display(Name = "Confirmar senha")]
    public string ConfirmarSenha { get; set; } = string.Empty;
}

public sealed class UsuarioRedefinirSenhaViewModel
{
    public int UsuarioID { get; set; }

    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a nova senha.")]
    [StringLength(
        100,
        MinimumLength = 8,
        ErrorMessage = "A senha deve possuir pelo menos 8 caracteres."
    )]
    [DataType(DataType.Password)]
    [Display(Name = "Nova senha")]
    public string NovaSenha { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirme a nova senha.")]
    [DataType(DataType.Password)]
    [Compare(
        nameof(NovaSenha),
        ErrorMessage = "A confirmação da senha não confere."
    )]
    [Display(Name = "Confirmar nova senha")]
    public string ConfirmarSenha { get; set; } = string.Empty;
}