using GestaoComercial.Web.Data;
using GestaoComercial.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace GestaoComercial.Web.Controllers;

public sealed class ProdutosController : Controller
{
    private readonly ProdutoRepository _produtoRepository;
    private readonly CategoriaRepository _categoriaRepository;

    public ProdutosController(
        ProdutoRepository produtoRepository,
        CategoriaRepository categoriaRepository
    )
    {
        _produtoRepository = produtoRepository;
        _categoriaRepository = categoriaRepository;
    }

    public async Task<IActionResult> Index()
    {
        var produtos = await _produtoRepository.ListarAsync();

        return View(produtos);
    }

    [HttpGet]
    public async Task<IActionResult> Criar()
    {
        await CarregarCategoriasAsync();

        return View(new ProdutoFormularioViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Criar(
        ProdutoFormularioViewModel produto
    )
    {
        if (!await CategoriaPodeSerUtilizadaAsync(produto.CategoriaID))
        {
            ModelState.AddModelError(
                nameof(produto.CategoriaID),
                "Selecione uma categoria ativa."
            );
        }

        if (!ModelState.IsValid)
        {
            await CarregarCategoriasAsync(produto.CategoriaID);

            return View(produto);
        }

        try
        {
            await _produtoRepository.InserirAsync(produto);

            TempData["MensagemSucesso"] =
                "Produto cadastrado com sucesso.";

            return RedirectToAction(nameof(Index));
        }
        catch (SqlException exception)
            when (exception.Number is 2601 or 2627)
        {
            ModelState.AddModelError(
                nameof(produto.CodigoBarras),
                "Já existe um produto com esse código de barras."
            );

            await CarregarCategoriasAsync(produto.CategoriaID);

            return View(produto);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var produto = await _produtoRepository.ObterPorIdAsync(id);

        if (produto is null)
        {
            return NotFound();
        }

        await CarregarCategoriasAsync(produto.CategoriaID);

        return View(produto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(
        int id,
        ProdutoFormularioViewModel produto
    )
    {
        if (id != produto.ProdutoID)
        {
            return BadRequest();
        }

        var produtoAtual =
            await _produtoRepository.ObterPorIdAsync(produto.ProdutoID);

        if (produtoAtual is null)
        {
            return NotFound();
        }

        if (!await CategoriaPodeSerUtilizadaAsync(
                produto.CategoriaID,
                produtoAtual.CategoriaID
            ))
        {
            ModelState.AddModelError(
                nameof(produto.CategoriaID),
                "Selecione uma categoria válida."
            );
        }

        if (!ModelState.IsValid)
        {
            await CarregarCategoriasAsync(produto.CategoriaID);

            return View(produto);
        }

        try
        {
            var produtoAtualizado =
                await _produtoRepository.AtualizarAsync(produto);

            if (!produtoAtualizado)
            {
                return NotFound();
            }

            TempData["MensagemSucesso"] =
                "Produto atualizado com sucesso.";

            return RedirectToAction(nameof(Index));
        }
        catch (SqlException exception)
            when (exception.Number is 2601 or 2627)
        {
            ModelState.AddModelError(
                nameof(produto.CodigoBarras),
                "Já existe um produto com esse código de barras."
            );

            await CarregarCategoriasAsync(produto.CategoriaID);

            return View(produto);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AlterarStatus(
        int id,
        bool ativo
    )
    {
        var produtoAlterado =
            await _produtoRepository.AlterarStatusAsync(id, ativo);

        if (!produtoAlterado)
        {
            return NotFound();
        }

        TempData["MensagemSucesso"] = ativo
            ? "Produto ativado com sucesso."
            : "Produto desativado com sucesso.";

        return RedirectToAction(nameof(Index));
    }

    private async Task CarregarCategoriasAsync(
        int? categoriaSelecionada = null
    )
    {
        var categorias = await _categoriaRepository.ListarAsync();

        ViewBag.Categorias = categorias
            .Where(categoria =>
                categoria.Ativo ||
                categoria.CategoriaID == categoriaSelecionada
            )
            .Select(categoria => new SelectListItem
            {
                Value = categoria.CategoriaID.ToString(),
                Text = categoria.Ativo
                    ? categoria.Nome
                    : $"{categoria.Nome} (inativa)",
                Selected =
                    categoria.CategoriaID == categoriaSelecionada
            })
            .ToList();
    }

    private async Task<bool> CategoriaPodeSerUtilizadaAsync(
        int categoriaID,
        int? categoriaAtualID = null
    )
    {
        var categorias = await _categoriaRepository.ListarAsync();

        return categorias.Any(categoria =>
            categoria.CategoriaID == categoriaID &&
            (
                categoria.Ativo ||
                categoria.CategoriaID == categoriaAtualID
            )
        );
    }
}