using GestaoComercial.Web.Data;
using GestaoComercial.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace GestaoComercial.Web.Controllers;

public sealed class EntradasController : Controller
{
    private readonly EntradaRepository _entradaRepository;

    public EntradasController(EntradaRepository entradaRepository)
    {
        _entradaRepository = entradaRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var entradas = await _entradaRepository.ListarAsync();

        return View(entradas);
    }

    [HttpGet]
    public async Task<IActionResult> Criar()
    {
        await CarregarOpcoesCriacaoAsync();

        return View(new EntradaCriarViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Criar(
        EntradaCriarViewModel entrada
    )
    {
        if (!ModelState.IsValid)
        {
            await CarregarOpcoesCriacaoAsync(
                entrada.FornecedorID,
                entrada.UsuarioID
            );

            return View(entrada);
        }

        try
        {
            var entradaID =
                await _entradaRepository.CriarAsync(entrada);

            TempData["MensagemSucesso"] =
                "Entrada criada. Agora adicione os produtos.";

            return RedirectToAction(
                nameof(Detalhes),
                new { id = entradaID }
            );
        }
        catch (SqlException exception)
            when (EhErroDeNegocio(exception))
        {
            ModelState.AddModelError(
                string.Empty,
                ObterMensagemErro(exception)
            );

            await CarregarOpcoesCriacaoAsync(
                entrada.FornecedorID,
                entrada.UsuarioID
            );

            return View(entrada);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Detalhes(int id)
    {
        var entrada = await _entradaRepository.ObterPorIdAsync(id);

        if (entrada is null)
        {
            return NotFound();
        }

        if (entrada.Status == "ABERTA")
        {
            await CarregarProdutosAsync();
        }

        return View(entrada);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AdicionarItem(
        AdicionarItemEntradaViewModel item
    )
    {
        if (!ModelState.IsValid)
        {
            TempData["MensagemErro"] =
                ObterPrimeiroErroValidacao();

            return RedirectToAction(
                nameof(Detalhes),
                new { id = item.EntradaID }
            );
        }

        try
        {
            await _entradaRepository.AdicionarItemAsync(item);

            TempData["MensagemSucesso"] =
                "Produto adicionado à entrada.";
        }
        catch (SqlException exception)
            when (EhErroDeNegocio(exception))
        {
            TempData["MensagemErro"] =
                ObterMensagemErro(exception);
        }

        return RedirectToAction(
            nameof(Detalhes),
            new { id = item.EntradaID }
        );
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AtualizarItem(
        int entradaID,
        int itemEntradaID,
        int quantidade,
        decimal custoUnitario
    )
    {
        if (quantidade <= 0)
        {
            TempData["MensagemErro"] =
                "A quantidade deve ser maior que zero.";

            return RedirectToAction(
                nameof(Detalhes),
                new { id = entradaID }
            );
        }

        if (custoUnitario < 0)
        {
            TempData["MensagemErro"] =
                "O custo unitário não pode ser negativo.";

            return RedirectToAction(
                nameof(Detalhes),
                new { id = entradaID }
            );
        }

        try
        {
            await _entradaRepository.AtualizarItemAsync(
                itemEntradaID,
                quantidade,
                custoUnitario
            );

            TempData["MensagemSucesso"] =
                "Item atualizado com sucesso.";
        }
        catch (SqlException exception)
            when (EhErroDeNegocio(exception))
        {
            TempData["MensagemErro"] =
                ObterMensagemErro(exception);
        }

        return RedirectToAction(
            nameof(Detalhes),
            new { id = entradaID }
        );
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoverItem(
        int entradaID,
        int itemEntradaID
    )
    {
        try
        {
            var entradaEncontradaID =
                await _entradaRepository.RemoverItemAsync(
                    itemEntradaID
                );

            TempData["MensagemSucesso"] =
                "Item removido da entrada.";

            return RedirectToAction(
                nameof(Detalhes),
                new { id = entradaEncontradaID }
            );
        }
        catch (SqlException exception)
            when (EhErroDeNegocio(exception))
        {
            TempData["MensagemErro"] =
                ObterMensagemErro(exception);

            return RedirectToAction(
                nameof(Detalhes),
                new { id = entradaID }
            );
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Confirmar(int id)
    {
        try
        {
            await _entradaRepository.ConfirmarAsync(id);

            TempData["MensagemSucesso"] =
                "Entrada confirmada e estoque atualizado.";
        }
        catch (SqlException exception)
            when (EhErroDeNegocio(exception))
        {
            TempData["MensagemErro"] =
                ObterMensagemErro(exception);
        }

        return RedirectToAction(
            nameof(Detalhes),
            new { id }
        );
    }

    private async Task CarregarOpcoesCriacaoAsync(
        int? fornecedorSelecionado = null,
        int? usuarioSelecionado = null
    )
    {
        var fornecedores =
            await _entradaRepository
                .ListarFornecedoresAtivosAsync();

        var usuarios =
            await _entradaRepository
                .ListarUsuariosAtivosAsync();

        ViewBag.Fornecedores = new SelectList(
            fornecedores,
            nameof(OpcaoSelecaoViewModel.ID),
            nameof(OpcaoSelecaoViewModel.Nome),
            fornecedorSelecionado
        );

        ViewBag.Usuarios = new SelectList(
            usuarios,
            nameof(OpcaoSelecaoViewModel.ID),
            nameof(OpcaoSelecaoViewModel.Nome),
            usuarioSelecionado
        );
    }

    private async Task CarregarProdutosAsync()
    {
        var produtos =
            await _entradaRepository
                .ListarProdutosAtivosAsync();

        ViewBag.Produtos = new SelectList(
            produtos,
            nameof(OpcaoSelecaoViewModel.ID),
            nameof(OpcaoSelecaoViewModel.Nome)
        );
    }

    private string ObterPrimeiroErroValidacao()
    {
        return ModelState.Values
            .SelectMany(valor => valor.Errors)
            .Select(erro => erro.ErrorMessage)
            .FirstOrDefault()
            ?? "Verifique os dados informados.";
    }

    private static bool EhErroDeNegocio(
        SqlException exception
    )
    {
        return exception.Number is >= 50001 and <= 50013;
    }

    private static string ObterMensagemErro(
        SqlException exception
    )
    {
        return exception.Number switch
        {
            50001 => "Fornecedor inexistente ou inativo.",
            50002 => "Usuário inexistente ou inativo.",
            50003 => "A quantidade deve ser maior que zero.",
            50004 => "O custo unitário não pode ser negativo.",
            50005 => "A entrada não existe ou não está aberta.",
            50006 => "Produto inexistente ou inativo.",
            50007 => "O produto já foi adicionado à entrada.",
            50008 => "A entrada não existe ou não está aberta.",
            50009 => "Adicione pelo menos um produto antes de confirmar.",
            50010 => "A quantidade deve ser maior que zero.",
            50011 => "O custo unitário não pode ser negativo.",
            50012 => "O item não existe ou a entrada não está aberta.",
            50013 => "O item não existe ou a entrada não está aberta.",
            _ => "Não foi possível concluir a operação."
        };
    }
}