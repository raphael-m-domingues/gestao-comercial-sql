using GestaoComercial.Web.Data;
using GestaoComercial.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace GestaoComercial.Web.Controllers;

public sealed class VendasController : Controller
{
    private readonly VendaRepository _vendaRepository;

    public VendasController(
        VendaRepository vendaRepository
    )
    {
        _vendaRepository = vendaRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var vendas =
            await _vendaRepository.ListarAsync();

        return View(vendas);
    }

    [HttpGet]
    public async Task<IActionResult> Criar()
    {
        await CarregarOpcoesCriacaoAsync();

        return View(new VendaCriarViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Criar(
        VendaCriarViewModel venda
    )
    {
        if (!ModelState.IsValid)
        {
            await CarregarOpcoesCriacaoAsync(
                venda.UsuarioID,
                venda.FormaPagamentoID
            );

            return View(venda);
        }

        try
        {
            var vendaID =
                await _vendaRepository.CriarAsync(venda);

            TempData["MensagemSucesso"] =
                "Venda criada. Agora adicione os produtos.";

            return RedirectToAction(
                nameof(Detalhes),
                new
                {
                    id = vendaID
                }
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
                venda.UsuarioID,
                venda.FormaPagamentoID
            );

            return View(venda);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Detalhes(int id)
    {
        var venda =
            await _vendaRepository.ObterPorIdAsync(id);

        if (venda is null)
        {
            return NotFound();
        }

        if (venda.Status == "ABERTA")
        {
            await CarregarProdutosAsync();
        }

        return View(venda);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AdicionarItem(
        AdicionarItemVendaViewModel item
    )
    {
        if (!ModelState.IsValid)
        {
            TempData["MensagemErro"] =
                ObterPrimeiroErroValidacao();

            return RedirectToAction(
                nameof(Detalhes),
                new
                {
                    id = item.VendaID
                }
            );
        }

        try
        {
            await _vendaRepository
                .AdicionarItemAsync(item);

            TempData["MensagemSucesso"] =
                "Produto adicionado à venda.";
        }
        catch (SqlException exception)
            when (EhErroDeNegocio(exception))
        {
            TempData["MensagemErro"] =
                ObterMensagemErro(exception);
        }

        return RedirectToAction(
            nameof(Detalhes),
            new
            {
                id = item.VendaID
            }
        );
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AtualizarItem(
        int vendaID,
        int itemVendaID,
        int quantidade
    )
    {
        if (quantidade <= 0)
        {
            TempData["MensagemErro"] =
                "A quantidade deve ser maior que zero.";

            return RedirectToAction(
                nameof(Detalhes),
                new
                {
                    id = vendaID
                }
            );
        }

        try
        {
            await _vendaRepository.AtualizarItemAsync(
                itemVendaID,
                quantidade
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
            new
            {
                id = vendaID
            }
        );
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoverItem(
        int vendaID,
        int itemVendaID
    )
    {
        try
        {
            var vendaEncontradaID =
                await _vendaRepository.RemoverItemAsync(
                    itemVendaID
                );

            TempData["MensagemSucesso"] =
                "Item removido da venda.";

            return RedirectToAction(
                nameof(Detalhes),
                new
                {
                    id = vendaEncontradaID
                }
            );
        }
        catch (SqlException exception)
            when (EhErroDeNegocio(exception))
        {
            TempData["MensagemErro"] =
                ObterMensagemErro(exception);

            return RedirectToAction(
                nameof(Detalhes),
                new
                {
                    id = vendaID
                }
            );
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Concluir(int id)
    {
        try
        {
            await _vendaRepository.ConcluirAsync(id);

            TempData["MensagemSucesso"] =
                "Venda concluída e estoque atualizado.";
        }
        catch (SqlException exception)
            when (EhErroDeNegocio(exception))
        {
            TempData["MensagemErro"] =
                ObterMensagemErro(exception);
        }

        return RedirectToAction(
            nameof(Detalhes),
            new
            {
                id
            }
        );
    }

    private async Task CarregarOpcoesCriacaoAsync(
        int? usuarioSelecionado = null,
        int? formaPagamentoSelecionada = null
    )
    {
        var usuarios =
            await _vendaRepository
                .ListarUsuariosAtivosAsync();

        var formasPagamento =
            await _vendaRepository
                .ListarFormasPagamentoAtivasAsync();

        ViewBag.Usuarios = new SelectList(
            usuarios,
            nameof(OpcaoSelecaoViewModel.ID),
            nameof(OpcaoSelecaoViewModel.Nome),
            usuarioSelecionado
        );

        ViewBag.FormasPagamento = new SelectList(
            formasPagamento,
            nameof(OpcaoSelecaoViewModel.ID),
            nameof(OpcaoSelecaoViewModel.Nome),
            formaPagamentoSelecionada
        );
    }

    private async Task CarregarProdutosAsync()
    {
        var produtos =
            await _vendaRepository
                .ListarProdutosDisponiveisAsync();

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
        return exception.Number is >= 52001 and <= 52014;
    }

    private static string ObterMensagemErro(
        SqlException exception
    )
    {
        return exception.Number switch
        {
            52001 =>
                "Usuário inexistente ou inativo.",

            52002 =>
                "Forma de pagamento inexistente ou inativa.",

            52003 =>
                "A quantidade deve ser maior que zero.",

            52004 =>
                "A venda não existe ou não está aberta.",

            52005 =>
                "Produto inexistente ou inativo.",

            52006 =>
                "Estoque insuficiente para adicionar o produto.",

            52007 =>
                "O produto já foi adicionado a esta venda.",

            52008 =>
                "A venda não existe ou não está aberta.",

            52009 =>
                "Adicione pelo menos um produto antes de concluir.",

            52010 =>
                "Estoque insuficiente para concluir a venda.",

            52011 =>
                "A quantidade deve ser maior que zero.",

            52012 =>
                "O item não existe ou a venda não está aberta.",

            52013 =>
                "Estoque insuficiente para atualizar o item.",

            52014 =>
                "O item não existe ou a venda não está aberta.",

            _ =>
                "Não foi possível concluir a operação."
        };
    }
}