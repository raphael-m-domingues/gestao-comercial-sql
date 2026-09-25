using System.Security.Claims;
using GestaoComercial.Web.Data;
using GestaoComercial.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GestaoComercial.Web.Controllers;

public sealed class AutenticacaoController : Controller
{
    private readonly AutenticacaoRepository
        _autenticacaoRepository;

    private readonly IPasswordHasher<UsuarioAutenticacaoViewModel>
        _passwordHasher;

    public AutenticacaoController(
        AutenticacaoRepository autenticacaoRepository,
        IPasswordHasher<UsuarioAutenticacaoViewModel>
            passwordHasher
    )
    {
        _autenticacaoRepository = autenticacaoRepository;
        _passwordHasher = passwordHasher;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction(
                "Index",
                "Home"
            );
        }

        return View(
            new LoginViewModel
            {
                ReturnUrl = returnUrl
            }
        );
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        LoginViewModel login
    )
    {
        if (!ModelState.IsValid)
        {
            return View(login);
        }

        var email =
            login.Email.Trim().ToLowerInvariant();

        var usuario =
            await _autenticacaoRepository
                .ObterPorEmailAsync(email);

        if (usuario is null || !usuario.Ativo)
        {
            AdicionarErroLogin();

            return View(login);
        }

        PasswordVerificationResult resultado;

        try
        {
            resultado =
                _passwordHasher.VerifyHashedPassword(
                    usuario,
                    usuario.SenhaHash,
                    login.Senha
                );
        }
        catch (FormatException)
        {
            resultado = PasswordVerificationResult.Failed;
        }

        if (resultado == PasswordVerificationResult.Failed)
        {
            AdicionarErroLogin();

            return View(login);
        }

        if (resultado
            == PasswordVerificationResult.SuccessRehashNeeded)
        {
            var novoHash =
                _passwordHasher.HashPassword(
                    usuario,
                    login.Senha
                );

            await _autenticacaoRepository
                .AtualizarSenhaHashAsync(
                    usuario.UsuarioID,
                    novoHash
                );
        }

        var claims = new List<Claim>
        {
            new(
                ClaimTypes.NameIdentifier,
                usuario.UsuarioID.ToString()
            ),
            new(
                ClaimTypes.Name,
                usuario.Nome
            ),
            new(
                ClaimTypes.Email,
                usuario.Email
            ),
            new(
                ClaimTypes.Role,
                usuario.Perfil
            )
        };

        var identidade = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        var principal = new ClaimsPrincipal(identidade);

        var propriedades =
            new AuthenticationProperties
            {
                IsPersistent = login.ManterConectado,
                AllowRefresh = true
            };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            propriedades
        );

        if (!string.IsNullOrWhiteSpace(login.ReturnUrl)
            && Url.IsLocalUrl(login.ReturnUrl))
        {
            return LocalRedirect(login.ReturnUrl);
        }

        return RedirectToAction(
            "Index",
            "Home"
        );
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Sair()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        return RedirectToAction(nameof(Login));
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult AcessoNegado()
    {
        return View();
    }

    private void AdicionarErroLogin()
    {
        ModelState.AddModelError(
            string.Empty,
            "E-mail ou senha inválidos."
        );
    }
}