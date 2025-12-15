using CashFlow.Exception;
using FluentAssertions;
using System.Text.Json;
using WebApi.Test.InlineData;

namespace WebApi.Test.Expenses.Delete;

public class DeleteExpenseByIdTest : CashFlowClassFixture
{
    private const string METHOD = "api/expenses";

    private readonly string _token;
    private readonly long _expenseId;
    public DeleteExpenseByIdTest(CustomWebApplicationFactory webApplicationFactory) : base(webApplicationFactory)
    {
        _token = webApplicationFactory.User_Team_Member.GetToken();
        _expenseId = webApplicationFactory.Expense_MemberTeam.GetId();
    }

    //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
    // Testes de unidade para o caso de uso DeleteExpenseUseCase
    //
    // Cenários: 
    // - Sucesso 
    // - Erro: Expense não encontrado
    //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*

    [Fact]
    public async Task Success()
    {
        //**********************************************
        // Testo o endpoint que deleta uma despesa
        //**********************************************
        var result = await DoDelete(requestUri: $"{METHOD}/{_expenseId}", token: _token);

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);

        //**********************************************
        // Verifico se ainda existe essa despesa
        // tentando buscar a mesma pela rota GET
        //**********************************************
        result = await DoGet(requestUri: $"{METHOD}/{_expenseId}", token: _token);

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_Expense_Not_Found(string culture)
    {
        var result = await DoDelete(requestUri: $"{METHOD}/0", token: _token, cultureInfo: culture);

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);

        var body = await result.Content.ReadAsStreamAsync();
        var response = await JsonDocument.ParseAsync(body);

        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();

        var expectedMessage = ResourceErrorMessages.ResourceManager.GetString("EXPENSE_NOT_FOUND",
            new System.Globalization.CultureInfo(culture));

        errors.Should().HaveCount(1).And.Contain(error => error.GetString()!.Equals(expectedMessage));
    }

}
