using GestaoComercial.Web.Data;
using Microsoft.AspNetCore.Mvc;

namespace GestaoComercial.Web.Controllers;

public sealed class EstoqueController : Controller
{
    private readonly EstoqueRepository _estoqueRepository;

    public EstoqueController(EstoqueRepository estoqueRepository)
    {
        _estoqueRepository = estoqueRepository;
    }

    public async Task<IActionResult> Index()
    {
        var produtos =
            await _estoqueRepository.ListarProdutosEstoqueBaixoAsync();

        return View(produtos);
    }
}