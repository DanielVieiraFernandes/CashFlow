using FluentAssertions;
using System.Text.Json;

namespace WebApi.Test.Expenses.GetAll;

public class GetAllExpensesTest : CashFlowClassFixture
{
    private const string METHOD = "api/expenses";
    private readonly string _token;
    public GetAllExpensesTest(CustomWebApplicationFactory webApplicationFactory) : base(webApplicationFactory)
    {
        _token = webApplicationFactory.GetToken();
    }

    [Fact]
    public async Task Success()
    {
        var result = await DoGet(METHOD, token: _token);

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var body = await result.Content.ReadAsStreamAsync();

        var response = await JsonDocument.ParseAsync(body);

        response.RootElement.GetProperty("expenses").EnumerateArray().Should().NotBeNullOrEmpty();
    }
}
