using GestaoComercial.Web.Data;
using GestaoComercial.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace GestaoComercial.Web.Controllers;

public sealed class RelatoriosController : Controller
{
    private readonly RelatorioRepository _relatorioRepository;

    public RelatoriosController(
        RelatorioRepository relatorioRepository
    )
    {
        _relatorioRepository = relatorioRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        DateTime? dataInicial,
        DateTime? dataFinal,
        int limite = 10
    )
    {
        var inicio =
            dataInicial?.Date
            ?? DateTime.Today.AddDays(-30);

        var fim =
            dataFinal?.Date
            ?? DateTime.Today;

        var relatorio = new RelatorioGerencialViewModel
        {
            DataInicial = inicio,
            DataFinal = fim,
            Limite = limite
        };

        if (inicio > fim)
        {
            ModelState.AddModelError(
                nameof(relatorio.DataInicial),
                "A data inicial não pode ser posterior à data final."
            );
        }

        if (limite is < 1 or > 100)
        {
            ModelState.AddModelError(
                nameof(relatorio.Limite),
                "O limite deve estar entre 1 e 100."
            );
        }

        if (!ModelState.IsValid)
        {
            relatorio.ResumoEstoque =
                await _relatorioRepository
                    .ObterResumoEstoqueAsync();

            return View(relatorio);
        }

        try
        {
            relatorio =
                await _relatorioRepository.ObterRelatorioAsync(
                    inicio,
                    fim,
                    limite
                );

            return View(relatorio);
        }
        catch (SqlException exception)
            when (EhErroDeNegocio(exception))
        {
            ModelState.AddModelError(
                string.Empty,
                ObterMensagemErro(exception)
            );

            relatorio.ResumoEstoque =
                await _relatorioRepository
                    .ObterResumoEstoqueAsync();

            return View(relatorio);
        }
    }

    private static bool EhErroDeNegocio(
        SqlException exception
    )
    {
        return exception.Number is >= 54001 and <= 54005;
    }

    private static string ObterMensagemErro(
        SqlException exception
    )
    {
        return exception.Number switch
        {
            54001 =>
                "As datas inicial e final são obrigatórias.",

            54002 =>
                "A data inicial não pode ser posterior à data final.",

            54003 =>
                "As datas inicial e final são obrigatórias.",

            54004 =>
                "A data inicial não pode ser posterior à data final.",

            54005 =>
                "O limite deve estar entre 1 e 100.",

            _ =>
                "Não foi possível gerar os relatórios."
        };
    }
}