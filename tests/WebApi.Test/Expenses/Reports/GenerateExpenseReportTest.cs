using FluentAssertions;
using System.Globalization;
using System.Net;
using System.Net.Mime;

namespace WebApi.Test.Expenses.Reports;

public class GenerateExpenseReportTest : CashFlowClassFixture
{
    private const string METHOD = "api/report";

    private readonly string _adminToken;
    private readonly string _teamMemberToken;
    private readonly DateTime _expenseDate;

    public GenerateExpenseReportTest(CustomWebApplicationFactory webApplicationFactory) : base(webApplicationFactory)
    {
        _adminToken = webApplicationFactory.User_Admin.GetToken();
        _teamMemberToken = webApplicationFactory.User_Team_Member.GetToken();
        _expenseDate = webApplicationFactory.Expense_Admin.GetDate();

        CultureInfo.CurrentCulture = new CultureInfo("pt-BR");
        CultureInfo.CurrentUICulture = new CultureInfo("pt-BR");
    }

    [Fact]
    public async Task Success_Pdf()
    {
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
        // _expenseDate:Y -> O 'Y' formata a data para mostrar somente o mês e o ano
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Tive que mudar o formato da data na query string para "yyyy-MM-dd" porque
        // o formato "Y" estava causando problemas ao interpretar a data no endpoint.
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        var result = await DoGet(requestUri: $"{METHOD}/pdf?month={_expenseDate:yyyy-MM-dd}", token: _adminToken,
            cultureInfo: "pt-BR");

        result.StatusCode.Should().Be(HttpStatusCode.OK);

        result.Content.Headers.ContentType.Should().NotBeNull();
        result.Content.Headers.ContentType!.MediaType.Should().Be(MediaTypeNames.Application.Pdf);
    }

    [Fact]
    public async Task Success_Excel()
    {
        var result = await DoGet(requestUri: $"{METHOD}/excel?month={_expenseDate:yyyy-MM-dd}", token: _adminToken, cultureInfo: "pt-BR");

        result.StatusCode.Should().Be(HttpStatusCode.OK);

        result.Content.Headers.ContentType.Should().NotBeNull();
        result.Content.Headers.ContentType!.MediaType.Should().Be(MediaTypeNames.Application.Octet);
    }

    [Fact]
    public async Task Error_Forbbinden_User_Not_Allowed_Pdf()
    {
        var result = await DoGet(requestUri: $"{METHOD}/pdf?month={_expenseDate:yyyy-MM-dd}", token: _teamMemberToken);

        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Error_Forbbinden_User_Not_Allowed_Excel()
    {
        var result = await DoGet(requestUri: $"{METHOD}/excel?month={_expenseDate:yyyy-MM-dd}", token: _teamMemberToken);

        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }



}
