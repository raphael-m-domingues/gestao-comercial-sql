namespace GestaoComercial.Web.Models;

public sealed class VendaConsultaViewModel
{
    public int? VendaID { get; set; }

    public int? FormaPagamentoID { get; set; }

    public string Status { get; set; } = "TODOS";

    public DateTime? DataInicial { get; set; }

    public DateTime? DataFinal { get; set; }

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

    public IEnumerable<VendaListagemViewModel> Vendas
        { get; set; } =
            Enumerable.Empty<VendaListagemViewModel>();
}