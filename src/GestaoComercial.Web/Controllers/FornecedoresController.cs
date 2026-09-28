using GestaoComercial.Web.Data;
using GestaoComercial.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace GestaoComercial.Web.Controllers;

[Authorize(Roles = "Administrador,Estoquista")]
public sealed class FornecedoresController : Controller
{
    private readonly FornecedorRepository
        _fornecedorRepository;

    public FornecedoresController(
        FornecedorRepository fornecedorRepository
    )
    {
        _fornecedorRepository = fornecedorRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var fornecedores =
            await _fornecedorRepository.ListarAsync();

        return View(fornecedores);
    }

    [HttpGet]
    public IActionResult Criar()
    {
        return View(new FornecedorViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Criar(
        FornecedorViewModel fornecedor
    )
    {
        if (!ModelState.IsValid)
        {
            return View(fornecedor);
        }

        NormalizarFornecedor(fornecedor);

        try
        {
            await _fornecedorRepository
                .InserirAsync(fornecedor);

            TempData["MensagemSucesso"] =
                "Fornecedor cadastrado com sucesso.";

            return RedirectToAction(nameof(Index));
        }
        catch (SqlException exception)
            when (exception.Number is 2601 or 2627)
        {
            ModelState.AddModelError(
                nameof(fornecedor.CNPJ),
                "Já existe um fornecedor com esse CNPJ."
            );

            return View(fornecedor);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var fornecedor =
            await _fornecedorRepository.ObterPorIdAsync(id);

        if (fornecedor is null)
        {
            return NotFound();
        }

        return View(fornecedor);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(
        int id,
        FornecedorViewModel fornecedor
    )
    {
        if (id != fornecedor.FornecedorID)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(fornecedor);
        }

        NormalizarFornecedor(fornecedor);

        try
        {
            var fornecedorAtualizado =
                await _fornecedorRepository
                    .AtualizarAsync(fornecedor);

            if (!fornecedorAtualizado)
            {
                return NotFound();
            }

            TempData["MensagemSucesso"] =
                "Fornecedor atualizado com sucesso.";

            return RedirectToAction(nameof(Index));
        }
        catch (SqlException exception)
            when (exception.Number is 2601 or 2627)
        {
            ModelState.AddModelError(
                nameof(fornecedor.CNPJ),
                "Já existe um fornecedor com esse CNPJ."
            );

            return View(fornecedor);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AlterarStatus(
        int id,
        bool ativo
    )
    {
        var fornecedorAtualizado =
            await _fornecedorRepository
                .AlterarStatusAsync(id, ativo);

        if (!fornecedorAtualizado)
        {
            return NotFound();
        }

        TempData["MensagemSucesso"] = ativo
            ? "Fornecedor ativado com sucesso."
            : "Fornecedor desativado com sucesso.";

        return RedirectToAction(nameof(Index));
    }

    private static void NormalizarFornecedor(
        FornecedorViewModel fornecedor
    )
    {
        fornecedor.RazaoSocial =
            fornecedor.RazaoSocial.Trim();

        fornecedor.NomeFantasia =
            NormalizarTextoOpcional(
                fornecedor.NomeFantasia
            );

        fornecedor.CNPJ =
            NormalizarNumeros(fornecedor.CNPJ);

        fornecedor.Telefone =
            NormalizarNumeros(fornecedor.Telefone);

        fornecedor.Email =
            NormalizarTextoOpcional(fornecedor.Email)?
                .ToLowerInvariant();
    }

    private static string? NormalizarTextoOpcional(
        string? valor
    )
    {
        return string.IsNullOrWhiteSpace(valor)
            ? null
            : valor.Trim();
    }

    private static string? NormalizarNumeros(
        string? valor
    )
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return null;
        }

        var numeros =
            new string(
                valor
                    .Where(char.IsDigit)
                    .ToArray()
            );

        return string.IsNullOrEmpty(numeros)
            ? null
            : numeros;
    }
}