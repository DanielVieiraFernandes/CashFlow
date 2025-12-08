using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace WebApi.Test;

//-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
// Classe base para testes de integração.
// Implementa IClassFixture com o servidor customizado
// de aplicação web para testes de integração.
// Contém métodos e propriedades comuns para os testes.
// Nos ajuda a evitar duplicação de código.
//-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
public class CashFlowClassFixture : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _httpClient;
    public CashFlowClassFixture(CustomWebApplicationFactory webApplicationFactory)
    {
        //===============================================================================
        // Esse servidor web simula a aplicação real durante os testes de integração.
        // e devolve um HttpClient configurado para fazer requisições a esse servidor.
        //===============================================================================
        _httpClient = webApplicationFactory.CreateClient();
    }

    //---------------------------------------------------------------
    // Métodos e propriedades comuns para os testes de integração
    //---------------------------------------------------------------

    /// <summary>
    /// Método auxiliar para realizar requisições POST com token e cultura.
    /// </summary>
    /// <param name="requestUri"></param>
    /// <param name="request"></param>
    /// <param name="token"></param>
    /// <param name="cultureInfo"></param>
    /// <returns></returns>
    protected async Task<HttpResponseMessage> DoPost(string requestUri,
        object request,
        string token = "",
        string cultureInfo = "")
    {

        //****************************************************************
        // Caso eu receba um token, adiciono no cabeçalho da requisição
        //****************************************************************
        if (!string.IsNullOrEmpty(token))
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //******************************************************************************
        // Caso eu receba uma cultura específica, adiciono no cabeçalho da requisição
        //******************************************************************************
        if (!string.IsNullOrEmpty(cultureInfo))
        {
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // Limpo os valores anteriores para não haver conflito
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++
            _httpClient.DefaultRequestHeaders.AcceptLanguage.Clear();
            _httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(cultureInfo));
        }

        return await _httpClient.PostAsJsonAsync(requestUri, request);
    }

}
