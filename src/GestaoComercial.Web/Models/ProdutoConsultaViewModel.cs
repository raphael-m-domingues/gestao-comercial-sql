namespace GestaoComercial.Web.Models;

public sealed class ProdutoConsultaViewModel
{
    public string? Busca { get; set; }

    public int? CategoriaID { get; set; }

    public string Status { get; set; } = "TODOS";

    public string SituacaoEstoque { get; set; } = "TODOS";

    public int Pagina { get; set; } = 1;

    public int TamanhoPagina { get; set; } = 10;

    public int TotalRegistros { get; set; }

    public int TotalPaginas =>
        TotalRegistros == 0
            ? 1
            : (int)Math.Ceiling(
                TotalRegistros / (double)TamanhoPagina
            );

    public int PrimeiroRegistro =>
        TotalRegistros == 0
            ? 0
            : ((Pagina - 1) * TamanhoPagina) + 1;

    public int UltimoRegistro =>
        Math.Min(
            Pagina * TamanhoPagina,
            TotalRegistros
        );

    public IEnumerable<ProdutoListagemViewModel> Produtos
        { get; set; } =
            Enumerable.Empty<ProdutoListagemViewModel>();
}