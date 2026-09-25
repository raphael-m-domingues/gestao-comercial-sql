using GestaoComercial.Web.Data;
using GestaoComercial.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace GestaoComercial.Web.Services;

public sealed class AdministradorInicializador
{
    private readonly AutenticacaoRepository
        _autenticacaoRepository;

    private readonly IPasswordHasher<UsuarioAutenticacaoViewModel>
        _passwordHasher;

    private readonly IConfiguration _configuration;

    public AdministradorInicializador(
        AutenticacaoRepository autenticacaoRepository,
        IPasswordHasher<UsuarioAutenticacaoViewModel>
            passwordHasher,
        IConfiguration configuration
    )
    {
        _autenticacaoRepository = autenticacaoRepository;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
    }

    public async Task InicializarAsync()
    {
        if (await _autenticacaoRepository
            .ExisteAdministradorAsync())
        {
            return;
        }

        var nome =
            _configuration["BootstrapAdmin:Nome"];

        var email =
            _configuration["BootstrapAdmin:Email"];

        var senha =
            _configuration["BootstrapAdmin:Senha"];

        if (string.IsNullOrWhiteSpace(nome)
            || string.IsNullOrWhiteSpace(email)
            || string.IsNullOrWhiteSpace(senha))
        {
            throw new InvalidOperationException(
                "Configure BootstrapAdmin nos User Secrets " +
                "antes de iniciar a aplicação."
            );
        }

        var administrador =
            new UsuarioAutenticacaoViewModel
            {
                Nome = nome.Trim(),
                Email = email.Trim().ToLowerInvariant(),
                Perfil = "Administrador",
                Ativo = true
            };

        var senhaHash =
            _passwordHasher.HashPassword(
                administrador,
                senha
            );

        await _autenticacaoRepository
            .CriarAdministradorAsync(
                administrador.Nome,
                administrador.Email,
                senhaHash
            );
    }
}