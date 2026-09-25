using System.Security.Claims;
using GestaoComercial.Web.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace GestaoComercial.Web.Services;

public sealed class AutenticacaoCookieEvents
    : CookieAuthenticationEvents
{
    private readonly AutenticacaoRepository
        _autenticacaoRepository;

    public AutenticacaoCookieEvents(
        AutenticacaoRepository autenticacaoRepository
    )
    {
        _autenticacaoRepository = autenticacaoRepository;
    }

    public override async Task ValidatePrincipal(
        CookieValidatePrincipalContext context
    )
    {
        var identificador =
            context.Principal?
                .FindFirstValue(ClaimTypes.NameIdentifier);

        var perfilCookie =
            context.Principal?
                .FindFirstValue(ClaimTypes.Role);

        if (!int.TryParse(identificador, out var usuarioID))
        {
            await RejeitarSessaoAsync(context);

            return;
        }

        var usuario =
            await _autenticacaoRepository
                .ObterUsuarioPorIdAsync(usuarioID);

        var sessaoValida =
            usuario is not null
            && usuario.Ativo
            && string.Equals(
                usuario.Perfil,
                perfilCookie,
                StringComparison.Ordinal
            );

        if (!sessaoValida)
        {
            await RejeitarSessaoAsync(context);
        }
    }

    private static async Task RejeitarSessaoAsync(
        CookieValidatePrincipalContext context
    )
    {
        context.RejectPrincipal();

        await context.HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme
        );
    }
}