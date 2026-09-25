using GestaoComercial.Web.Data;
using GestaoComercial.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace GestaoComercial.Web.Controllers;

[Authorize(Roles = "Administrador,Estoquista")]
public sealed class CategoriasController : Controller
{
    private readonly CategoriaRepository _categoriaRepository;

    public CategoriasController(
        CategoriaRepository categoriaRepository
    )
    {
        _categoriaRepository = categoriaRepository;
    }

    public async Task<IActionResult> Index()
    {
        var categorias =
            await _categoriaRepository.ListarAsync();

        return View(categorias);
    }

    [HttpGet]
    public IActionResult Criar()
    {
        return View(new CategoriaViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Criar(
        CategoriaViewModel categoria
    )
    {
        if (!ModelState.IsValid)
        {
            return View(categoria);
        }

        try
        {
            await _categoriaRepository.InserirAsync(categoria);

            TempData["Sucesso"] =
                "Categoria cadastrada com sucesso.";

            return RedirectToAction(nameof(Index));
        }
        catch (SqlException exception)
            when (exception.Number is 2601 or 2627)
        {
            ModelState.AddModelError(
                nameof(categoria.Nome),
                "Já existe uma categoria com esse nome."
            );

            return View(categoria);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var categoria =
            await _categoriaRepository.ObterPorIdAsync(id);

        if (categoria is null)
        {
            return NotFound();
        }

        return View(categoria);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(
        int id,
        CategoriaViewModel categoria
    )
    {
        if (id != categoria.CategoriaID)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(categoria);
        }

        try
        {
            var atualizada =
                await _categoriaRepository
                    .AtualizarAsync(categoria);

            if (!atualizada)
            {
                return NotFound();
            }

            TempData["Sucesso"] =
                "Categoria atualizada com sucesso.";

            return RedirectToAction(nameof(Index));
        }
        catch (SqlException exception)
            when (exception.Number is 2601 or 2627)
        {
            ModelState.AddModelError(
                nameof(categoria.Nome),
                "Já existe uma categoria com esse nome."
            );

            return View(categoria);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AlterarStatus(
        int id,
        bool ativo
    )
    {
        var atualizado =
            await _categoriaRepository.AlterarStatusAsync(
                id,
                ativo
            );

        if (!atualizado)
        {
            return NotFound();
        }

        TempData["Sucesso"] = ativo
            ? "Categoria ativada com sucesso."
            : "Categoria desativada com sucesso.";

        return RedirectToAction(nameof(Index));
    }
}