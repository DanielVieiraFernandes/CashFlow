using CashFlow.Exception;
using CommonTestUtilities.Requests;
using FluentAssertions;
using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebApi.Test.Users.Register;

//-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
// Para indicar que esta classe contém testes de integração,
// implementamos a interface IClassFixture<T> fornecida pelo xUnit.
// Passamos o CustomWebApplicationFactory para indicar o ambiente
// no qual a API Web será executada durante os testes.
//-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
public class RegisterUserTest : IClassFixture<CustomWebApplicationFactory>
{
    private const string METHOD = "api/user";
    private readonly HttpClient _httpClient;
    public RegisterUserTest(CustomWebApplicationFactory webApplicationFactory)
    {
        //===============================================================================
        // Esse servidor web simula a aplicação real durante os testes de integração.
        // e devolve um HttpClient configurado para fazer requisições a esse servidor.
        //===============================================================================

        _httpClient = webApplicationFactory.CreateClient();

        var cultureInfo = new CultureInfo("pt-BR");

        CultureInfo.CurrentCulture = cultureInfo;
        CultureInfo.CurrentUICulture = cultureInfo;
    }

    /*
     * Sobre testes de integração:
     * -*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
     * Testes de integração focam em verificar se diferentes partes do sistema 
     * funcionam corretamente quando integradas.Nesse caso, a API Web interage
     * com o banco de dados, serviços externos e outros componentes.
     * -*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
     * Como queremos testar diferentes componentes do sistema trabalhando juntos,
     * Utilizamos o WebApplicationFactory fornecido pelo 'Microsoft.AspNetCore.Mvc.Testing'
     * para criar um ambiente de teste que simula a aplicação real.
     * -*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
     * Ao executar testes de integração, a API não deve persistir dados 
     * no banco de dados real, mas sim em um banco de dados temporário ou
     * em memória, para evitar poluição de dados.
     * Pensando nisso, devemos configurar apenas um ambiente de testes isolado.
     * -*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
     */

    //**************************************************************************
    // Ambiente de testes:
    // A API irá ser executada em um ambiente isolado, onde podemos configurar
    // serviços específicos para testes, como um banco de dados em memória.
    // Para criar um ambiente de testes de integração, precisamos configurar o
    // WebApplicationFactory para usar um ambiente de testes.
    //**************************************************************************

    [Fact]
    public async Task Success()
    {
        var request = RequestRegisterUserJsonBuilder.Build();

        var result = await _httpClient.PostAsJsonAsync(METHOD, request);

        result.StatusCode.Should().Be(HttpStatusCode.Created);

        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Não é recomendado desserializar o retorno, pois
        // o objetivo dos testes de integração é validar a
        // resposta simulando um cliente real, logo
        // testaremos as propriedades do response diretamente.
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        // Lê o conteúdo da resposta como um stream
        var body = await result.Content.ReadAsStreamAsync();

        // Desserializar o stream para um JsonDocument
        // Agora temos a resposta em documento JSON para validar
        var response = await JsonDocument.ParseAsync(body);

        response.RootElement.GetProperty("name").GetString().Should().Be(request.Name);
        response.RootElement.GetProperty("token").GetString().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Error_Empty_Name()
    {
        var request = RequestRegisterUserJsonBuilder.Build();
        request.Name = string.Empty;

        var result = await _httpClient.PostAsJsonAsync(METHOD, request);

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var body = await result.Content.ReadAsStreamAsync();

        var response = await JsonDocument.ParseAsync(body);

        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();

        errors.Should().HaveCount(1).And.Contain(error => error.GetString()!.Equals(ResourceErrorMessages.NAME_EMPTY));
    }
}