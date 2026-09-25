using GestaoComercial.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoComercial.Web.Controllers;

[Authorize(Roles = "Administrador,Estoquista")]
public sealed class EstoqueController : Controller
{
    private readonly EstoqueRepository _estoqueRepository;

    public EstoqueController(
        EstoqueRepository estoqueRepository
    )
    {
        _estoqueRepository = estoqueRepository;
    }

    public async Task<IActionResult> Index()
    {
        var produtos =
            await _estoqueRepository
                .ListarProdutosEstoqueBaixoAsync();

        return View(produtos);
    }
}