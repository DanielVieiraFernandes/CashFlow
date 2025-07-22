using System.Globalization;

namespace CashFlow.Api.Middleware;

public class CultureMiddleware
{
    private readonly RequestDelegate next;
    public CultureMiddleware(RequestDelegate next)
    {
        this.next = next;
    }
    public async Task Invoke(HttpContext context)
    {
        var supportedLanguages = CultureInfo.GetCultures(CultureTypes.AllCultures).ToList();

        var requestedCulture = context.Request.Headers.AcceptLanguage.FirstOrDefault();

        var cultureInfo = new CultureInfo("en"); // Defino uma cultura padrão

        if (!string.IsNullOrWhiteSpace(requestedCulture) && supportedLanguages.Exists(l => l.Name.Equals(requestedCulture)))
        {
            cultureInfo = new CultureInfo(requestedCulture); // altero a cultura se tiver sido passado um idioma
        }

        CultureInfo.CurrentCulture = cultureInfo;
        CultureInfo.CurrentUICulture = cultureInfo;

        await next(context);
    }
}
