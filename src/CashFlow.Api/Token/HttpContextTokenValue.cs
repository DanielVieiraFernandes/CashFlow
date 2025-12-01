using CashFlow.Domain.Security.Tokens;

namespace CashFlow.Api.Token;

/// <summary>
/// 
/// </summary>
public class HttpContextTokenValue : ITokenProvider
{
    private readonly IHttpContextAccessor _contextAccessor;
    public HttpContextTokenValue(IHttpContextAccessor httpContextAccessor)
    {
        _contextAccessor = httpContextAccessor;
    }
    public string TokenOnRequest()
    {
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
        // Recupero o authorization do cabeçalho da requisição
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
        var authorization = _contextAccessor.HttpContext!.Request.Headers.Authorization.ToString();

        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
        // Formato de um token vindo da requisição:
        // "Bearer 123abc"
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*

        //*******************************************************************************
        // "Bearer ".Length: obtém o tamanho do prefixo.
        // O operador [..] fatia a string a partir dessa posição (o tamanho do prefixo)
        // até o final, retornando apenas o token limpo.
        //*******************************************************************************
        return authorization["Bearer ".Length..].Trim();
    }
}
