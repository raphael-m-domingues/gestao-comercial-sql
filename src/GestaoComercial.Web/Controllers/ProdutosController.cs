using GestaoComercial.Web.Data;
using GestaoComercial.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace GestaoComercial.Web.Controllers;

[Authorize(Roles = "Administrador,Estoquista,Caixa")]
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

    [HttpGet]
    public async Task<IActionResult> Index(
        ProdutoConsultaViewModel consulta
    )
    {
        NormalizarConsulta(consulta);

        var resultado =
            await _produtoRepository.ConsultarAsync(consulta);

        if (resultado.TotalRegistros > 0
            && resultado.Pagina > resultado.TotalPaginas)
        {
            resultado.Pagina = resultado.TotalPaginas;

            resultado =
                await _produtoRepository
                    .ConsultarAsync(resultado);
        }

        await CarregarCategoriasFiltroAsync(
            resultado.CategoriaID
        );

        return View(resultado);
    }

    [Authorize(Roles = "Administrador,Estoquista")]
    [HttpGet]
    public async Task<IActionResult> Criar()
    {
        await CarregarCategoriasAsync();

        return View(new ProdutoFormularioViewModel());
    }

    [Authorize(Roles = "Administrador,Estoquista")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Criar(
        ProdutoFormularioViewModel produto
    )
    {
        if (!await CategoriaPodeSerUtilizadaAsync(
                produto.CategoriaID
            ))
        {
            ModelState.AddModelError(
                nameof(produto.CategoriaID),
                "Selecione uma categoria ativa."
            );
        }

        if (!ModelState.IsValid)
        {
            await CarregarCategoriasAsync(
                produto.CategoriaID
            );

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

            await CarregarCategoriasAsync(
                produto.CategoriaID
            );

            return View(produto);
        }
    }

    [Authorize(Roles = "Administrador,Estoquista")]
    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var produto =
            await _produtoRepository.ObterPorIdAsync(id);

        if (produto is null)
        {
            return NotFound();
        }

        await CarregarCategoriasAsync(
            produto.CategoriaID
        );

        return View(produto);
    }

    [Authorize(Roles = "Administrador,Estoquista")]
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
            await _produtoRepository
                .ObterPorIdAsync(produto.ProdutoID);

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
            await CarregarCategoriasAsync(
                produto.CategoriaID
            );

            return View(produto);
        }

        try
        {
            var produtoAtualizado =
                await _produtoRepository
                    .AtualizarAsync(produto);

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

            await CarregarCategoriasAsync(
                produto.CategoriaID
            );

            return View(produto);
        }
    }

    [Authorize(Roles = "Administrador,Estoquista")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AlterarStatus(
        int id,
        string? busca,
        int? categoriaID,
        string status = "TODOS",
        string situacaoEstoque = "TODOS",
        int pagina = 1,
        int tamanhoPagina = 10
    )
    {
        var produto =
            await _produtoRepository.ObterPorIdAsync(id);

        if (produto is null)
        {
            return NotFound();
        }

        var novoStatus = !produto.Ativo;

        var produtoAlterado =
            await _produtoRepository
                .AlterarStatusAsync(id, novoStatus);

        if (!produtoAlterado)
        {
            return NotFound();
        }

        TempData["MensagemSucesso"] = novoStatus
            ? "Produto ativado com sucesso."
            : "Produto desativado com sucesso.";

        return RedirectToAction(
            nameof(Index),
            new
            {
                busca,
                categoriaID,
                status,
                situacaoEstoque,
                pagina,
                tamanhoPagina
            }
        );
    }

    private async Task CarregarCategoriasFiltroAsync(
        int? categoriaSelecionada
    )
    {
        var categorias =
            await _categoriaRepository.ListarAsync();

        ViewBag.CategoriasFiltro =
            new SelectList(
                categorias,
                nameof(CategoriaViewModel.CategoriaID),
                nameof(CategoriaViewModel.Nome),
                categoriaSelecionada
            );
    }

    private async Task CarregarCategoriasAsync(
        int? categoriaSelecionada = null
    )
    {
        var categorias =
            await _categoriaRepository.ListarAsync();

        ViewBag.Categorias = categorias
            .Where(categoria =>
                categoria.Ativo
                || categoria.CategoriaID
                    == categoriaSelecionada
            )
            .Select(categoria => new SelectListItem
            {
                Value =
                    categoria.CategoriaID.ToString(),

                Text = categoria.Ativo
                    ? categoria.Nome
                    : $"{categoria.Nome} (inativa)",

                Selected =
                    categoria.CategoriaID
                    == categoriaSelecionada
            })
            .ToList();
    }

    private async Task<bool> CategoriaPodeSerUtilizadaAsync(
        int categoriaID,
        int? categoriaAtualID = null
    )
    {
        var categorias =
            await _categoriaRepository.ListarAsync();

        return categorias.Any(categoria =>
            categoria.CategoriaID == categoriaID
            &&
            (
                categoria.Ativo
                || categoria.CategoriaID
                    == categoriaAtualID
            )
        );
    }

    private static void NormalizarConsulta(
        ProdutoConsultaViewModel consulta
    )
    {
        consulta.Busca =
            string.IsNullOrWhiteSpace(consulta.Busca)
                ? null
                : consulta.Busca.Trim();

        consulta.Pagina =
            Math.Max(consulta.Pagina, 1);

        if (consulta.TamanhoPagina
            is not (10 or 25 or 50))
        {
            consulta.TamanhoPagina = 10;
        }

        consulta.Status =
            consulta.Status?.Trim().ToUpperInvariant()
            switch
            {
                "ATIVOS" => "ATIVOS",
                "INATIVOS" => "INATIVOS",
                _ => "TODOS"
            };

        consulta.SituacaoEstoque =
            consulta.SituacaoEstoque?
                .Trim()
                .ToUpperInvariant()
            switch
            {
                "SEM_ESTOQUE" => "SEM_ESTOQUE",
                "ESTOQUE_BAIXO" => "ESTOQUE_BAIXO",
                "NORMAL" => "NORMAL",
                _ => "TODOS"
            };
    }
}