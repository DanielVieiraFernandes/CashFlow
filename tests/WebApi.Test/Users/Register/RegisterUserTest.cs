using CommonTestUtilities.Requests;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace WebApi.Test.Users.Register;

//-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
// Para indicar que esta classe contém testes de integração,
// implementamos a interface IClassFixture<T> fornecida pelo xUnit.
// Passamos o WebApplicationFactory para indicar onde a API irá 
// rodar durante os testes.
// Passamos o Program como parâmetro genérico, que é a classe
// principal da aplicação ASP.NET Core, o ponto de entrada.
//-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
public class RegisterUserTest : IClassFixture<WebApplicationFactory<Program>>
{
    private const string METHOD = "api/user";
    private readonly HttpClient _httpClient;
    public RegisterUserTest(WebApplicationFactory<Program> webApplicationFactory)
    {
        //===============================================================================
        // Esse servidor web simula a aplicação real durante os testes de integração.
        // e devolve um HttpClient configurado para fazer requisições a esse servidor.
        //===============================================================================

        _httpClient = webApplicationFactory.CreateClient();
    }

    /*
     * Sobre testes de integração:
     * -*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
     * Testes de integração focam em verificar se diferentes partes do sistema 
     * funcionam corretamente quando integradas.
     * Nesse caso, a API Web interage com o banco de dados, serviços externos
     * e outros componentes.
     * -*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
     * Como queremos testar diferentes componentes do sistema trabalhando juntos,
     * Utilizamos o WebApplicationFactory fornecido pelo xUnit para criar um 
     * ambiente de teste que simula a aplicação real.
     * -*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
     */

    [Fact]
    public async Task Success()
    {
        var request = RequestRegisterUserJsonBuilder.Build();

        var response = await _httpClient.PostAsJsonAsync(METHOD, request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
