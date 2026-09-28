using GestaoComercial.Web.Data;
using GestaoComercial.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace GestaoComercial.Web.Controllers;

[Authorize(Roles = "Administrador,Estoquista")]
public sealed class MovimentacoesEstoqueController : Controller
{
    private static readonly string[] TiposPermitidos =
    [
        "ENTRADA",
        "SAIDA",
        "DEVOLUCAO",
        "AJUSTE"
    ];

    private readonly MovimentacaoEstoqueRepository
        _movimentacaoRepository;

    public MovimentacoesEstoqueController(
        MovimentacaoEstoqueRepository movimentacaoRepository
    )
    {
        _movimentacaoRepository = movimentacaoRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        DateTime? dataInicial,
        DateTime? dataFinal,
        int? produtoID,
        int? usuarioID,
        string? tipo,
        int limite = 200
    )
    {
        tipo = string.IsNullOrWhiteSpace(tipo)
            ? null
            : tipo.Trim().ToUpperInvariant();

        var filtro = new MovimentacaoEstoqueFiltroViewModel
        {
            DataInicial = dataInicial?.Date,
            DataFinal = dataFinal?.Date,
            ProdutoID = produtoID,
            UsuarioID = usuarioID,
            Tipo = tipo,
            Limite = limite
        };

        if (dataInicial.HasValue
            && dataFinal.HasValue
            && dataInicial.Value.Date > dataFinal.Value.Date)
        {
            ModelState.AddModelError(
                nameof(filtro.DataInicial),
                "A data inicial não pode ser posterior à data final."
            );
        }

        if (tipo is not null
            && !TiposPermitidos.Contains(tipo))
        {
            ModelState.AddModelError(
                nameof(filtro.Tipo),
                "O tipo de movimentação informado é inválido."
            );
        }

        if (limite is < 1 or > 1000)
        {
            ModelState.AddModelError(
                nameof(filtro.Limite),
                "O limite deve estar entre 1 e 1000."
            );
        }

        if (ModelState.IsValid)
        {
            try
            {
                filtro.Movimentacoes =
                    (await _movimentacaoRepository.ListarAsync(
                        filtro.DataInicial,
                        filtro.DataFinal,
                        filtro.ProdutoID,
                        filtro.UsuarioID,
                        filtro.Tipo,
                        filtro.Limite
                    ))
                    .ToList();
            }
            catch (SqlException exception)
                when (EhErroDeNegocio(exception))
            {
                ModelState.AddModelError(
                    string.Empty,
                    ObterMensagemErro(exception)
                );
            }
        }

        await CarregarFiltrosAsync(filtro);

        return View(filtro);
    }

    private async Task CarregarFiltrosAsync(
        MovimentacaoEstoqueFiltroViewModel filtro
    )
    {
        var produtos =
            await _movimentacaoRepository.ListarProdutosAsync();

        var usuarios =
            await _movimentacaoRepository.ListarUsuariosAsync();

        ViewBag.Produtos = new SelectList(
            produtos,
            nameof(OpcaoSelecaoViewModel.ID),
            nameof(OpcaoSelecaoViewModel.Nome),
            filtro.ProdutoID
        );

        ViewBag.Usuarios = new SelectList(
            usuarios,
            nameof(OpcaoSelecaoViewModel.ID),
            nameof(OpcaoSelecaoViewModel.Nome),
            filtro.UsuarioID
        );

        ViewBag.Tipos = TiposPermitidos
            .Select(tipo => new SelectListItem
            {
                Value = tipo,
                Text = ObterDescricaoTipo(tipo),
                Selected = tipo == filtro.Tipo
            })
            .ToList();
    }

    private static string ObterDescricaoTipo(string tipo)
    {
        return tipo switch
        {
            "ENTRADA" => "Entrada",
            "SAIDA" => "Saída",
            "DEVOLUCAO" => "Devolução",
            "AJUSTE" => "Ajuste",
            _ => tipo
        };
    }

    private static bool EhErroDeNegocio(
        SqlException exception
    )
    {
        return exception.Number is >= 56001 and <= 56003;
    }

    private static string ObterMensagemErro(
        SqlException exception
    )
    {
        return exception.Number switch
        {
            56001 =>
                "A data inicial não pode ser posterior à data final.",

            56002 =>
                "O tipo de movimentação informado é inválido.",

            56003 =>
                "O limite deve estar entre 1 e 1000.",

            _ =>
                "Não foi possível consultar as movimentações."
        };
    }
}