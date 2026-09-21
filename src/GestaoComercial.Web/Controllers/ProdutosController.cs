using GestaoComercial.Web.Data;
using Microsoft.AspNetCore.Mvc;

namespace GestaoComercial.Web.Controllers;

public sealed class ProdutosController : Controller
{
    private readonly ProdutoRepository _produtoRepository;

    public ProdutosController(ProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    public async Task<IActionResult> Index()
    {
        var produtos = await _produtoRepository.ListarAsync();

        return View(produtos);
    }
}