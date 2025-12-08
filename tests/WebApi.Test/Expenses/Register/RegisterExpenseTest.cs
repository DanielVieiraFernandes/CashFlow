using CashFlow.Exception;
using CommonTestUtilities.Requests;
using FluentAssertions;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using WebApi.Test.InlineData;

namespace WebApi.Test.Expenses.Register;

//**************************************************************************************************
// Como essa classe é um teste de integração, devemos utilizar essa interface 'IClassFixture' 
// para configurar o ambiente de teste com a aplicação web.
//**************************************************************************************************
public class RegisterExpenseTest : IClassFixture<CustomWebApplicationFactory>
{
    private const string METHOD = "/api/expenses";

    private readonly HttpClient _httpClient;
    private readonly string _token;

    public RegisterExpenseTest(CustomWebApplicationFactory webApplicationFactory)
    {
        _httpClient = webApplicationFactory.CreateClient();

        //*****************************************************************
        // Recupero o token JWT criado no momento em que o usuário foi
        // registrado no banco de dados memória
        //*****************************************************************
        _token = webApplicationFactory.GetToken();
    }

    [Fact]
    public async Task Success()
    {
        var request = RequestExpenseJsonBuilder.Build();

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

        var result = await _httpClient.PostAsJsonAsync(METHOD, request);

        result.StatusCode.Should().Be(HttpStatusCode.Created);

        //++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Recupero o corpo da resposta e converto para JSON
        //++++++++++++++++++++++++++++++++++++++++++++++++++++
        var body = await result.Content.ReadAsStreamAsync();
        var response = await JsonDocument.ParseAsync(body);

        response.RootElement.GetProperty("title").GetString().Should().Be(request.Title);
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_Title_Name(string cultureInfo)
    {
        var request = RequestExpenseJsonBuilder.Build();
        request.Title = string.Empty;

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        _httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(cultureInfo));

        var result = await _httpClient.PostAsJsonAsync(METHOD, request);

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var body = await result.Content.ReadAsStreamAsync();
        var response = await JsonDocument.ParseAsync(body);

        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();

        var expectedMessage = ResourceErrorMessages.ResourceManager.GetString("TITLE_REQUIRED",
            new CultureInfo(cultureInfo));

        errors.Should().HaveCount(1).And.Contain(error => error.GetString()!.Equals(expectedMessage));
    }
}
