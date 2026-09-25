using System.Security.Claims;
using GestaoComercial.Web.Data;
using GestaoComercial.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace GestaoComercial.Web.Controllers;

[Authorize(Roles = "Administrador")]
public sealed class UsuariosController : Controller
{
    private readonly AutenticacaoRepository
        _autenticacaoRepository;

    private readonly IPasswordHasher<UsuarioAutenticacaoViewModel>
        _passwordHasher;

    public UsuariosController(
        AutenticacaoRepository autenticacaoRepository,
        IPasswordHasher<UsuarioAutenticacaoViewModel>
            passwordHasher
    )
    {
        _autenticacaoRepository = autenticacaoRepository;
        _passwordHasher = passwordHasher;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var usuarios =
            await _autenticacaoRepository
                .ListarUsuariosAsync();

        return View(usuarios);
    }

    [HttpGet]
    public async Task<IActionResult> Criar()
    {
        await CarregarPerfisAsync();

        return View(new UsuarioCriarViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Criar(
        UsuarioCriarViewModel usuario
    )
    {
        var perfis =
            (await _autenticacaoRepository
                .ListarPerfisAtivosAsync())
            .ToList();

        if (!perfis.Any(perfil =>
                perfil.ID == usuario.PerfilID))
        {
            ModelState.AddModelError(
                nameof(usuario.PerfilID),
                "Selecione um perfil ativo."
            );
        }

        if (!ModelState.IsValid)
        {
            CarregarPerfis(
                perfis,
                usuario.PerfilID
            );

            return View(usuario);
        }

        var usuarioHash =
            new UsuarioAutenticacaoViewModel
            {
                Nome = usuario.Nome.Trim(),
                Email =
                    usuario.Email.Trim().ToLowerInvariant(),
                Perfil =
                    perfis.Single(perfil =>
                        perfil.ID == usuario.PerfilID
                    ).Nome,
                Ativo = true
            };

        var senhaHash =
            _passwordHasher.HashPassword(
                usuarioHash,
                usuario.Senha
            );

        try
        {
            await _autenticacaoRepository
                .CriarUsuarioAsync(
                    usuario,
                    senhaHash
                );

            TempData["MensagemSucesso"] =
                "Usuário cadastrado com sucesso.";

            return RedirectToAction(nameof(Index));
        }
        catch (SqlException exception)
            when (exception.Number is 2601 or 2627)
        {
            ModelState.AddModelError(
                nameof(usuario.Email),
                "Já existe um usuário com esse e-mail."
            );

            CarregarPerfis(
                perfis,
                usuario.PerfilID
            );

            return View(usuario);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AlterarStatus(
        int id,
        bool ativo
    )
    {
        var usuarioAtualID =
            ObterUsuarioAtualID();

        if (usuarioAtualID == id && !ativo)
        {
            TempData["MensagemErro"] =
                "Você não pode desativar sua própria conta.";

            return RedirectToAction(nameof(Index));
        }

        var usuarioAlterado =
            await _autenticacaoRepository
                .AlterarStatusAsync(id, ativo);

        if (!usuarioAlterado)
        {
            return NotFound();
        }

        TempData["MensagemSucesso"] = ativo
            ? "Usuário ativado com sucesso."
            : "Usuário desativado com sucesso.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> RedefinirSenha(int id)
    {
        var usuario =
            await _autenticacaoRepository
                .ObterUsuarioPorIdAsync(id);

        if (usuario is null)
        {
            return NotFound();
        }

        return View(
            new UsuarioRedefinirSenhaViewModel
            {
                UsuarioID = usuario.UsuarioID,
                Nome = usuario.Nome
            }
        );
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RedefinirSenha(
        int id,
        UsuarioRedefinirSenhaViewModel modelo
    )
    {
        if (id != modelo.UsuarioID)
        {
            return BadRequest();
        }

        var usuario =
            await _autenticacaoRepository
                .ObterUsuarioPorIdAsync(id);

        if (usuario is null)
        {
            return NotFound();
        }

        modelo.Nome = usuario.Nome;

        if (!ModelState.IsValid)
        {
            return View(modelo);
        }

        var usuarioHash =
            new UsuarioAutenticacaoViewModel
            {
                UsuarioID = usuario.UsuarioID,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Perfil = usuario.Perfil,
                Ativo = usuario.Ativo
            };

        var senhaHash =
            _passwordHasher.HashPassword(
                usuarioHash,
                modelo.NovaSenha
            );

        await _autenticacaoRepository
            .AtualizarSenhaHashAsync(
                usuario.UsuarioID,
                senhaHash
            );

        TempData["MensagemSucesso"] =
            "Senha redefinida com sucesso.";

        return RedirectToAction(nameof(Index));
    }

    private async Task CarregarPerfisAsync(
        int? perfilSelecionado = null
    )
    {
        var perfis =
            await _autenticacaoRepository
                .ListarPerfisAtivosAsync();

        ViewBag.Perfis = new SelectList(
            perfis,
            nameof(OpcaoSelecaoViewModel.ID),
            nameof(OpcaoSelecaoViewModel.Nome),
            perfilSelecionado
        );
    }

    private void CarregarPerfis(
        IEnumerable<OpcaoSelecaoViewModel> perfis,
        int? perfilSelecionado = null
    )
    {
        ViewBag.Perfis = new SelectList(
            perfis,
            nameof(OpcaoSelecaoViewModel.ID),
            nameof(OpcaoSelecaoViewModel.Nome),
            perfilSelecionado
        );
    }

    private int ObterUsuarioAtualID()
    {
        var valor =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );

        return int.TryParse(valor, out var usuarioID)
            ? usuarioID
            : 0;
    }
}