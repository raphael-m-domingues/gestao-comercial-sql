using System.Diagnostics;
using GestaoComercial.Web.Data;
using GestaoComercial.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestaoComercial.Web.Controllers;

public sealed class HomeController : Controller
{
    private readonly DashboardRepository _dashboardRepository;

    public HomeController(DashboardRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }

    public async Task<IActionResult> Index()
    {
        var resumo = await _dashboardRepository.ObterResumoAsync();

        return View(resumo);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(
        Duration = 0,
        Location = ResponseCacheLocation.None,
        NoStore = true)]
    public IActionResult Error()
    {
        var modelo = new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        };

        return View(modelo);
    }
}