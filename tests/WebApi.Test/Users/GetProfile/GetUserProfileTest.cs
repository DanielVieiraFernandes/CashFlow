using FluentAssertions;
using System.Net;
using System.Text.Json;

namespace WebApi.Test.Users.GetProfile;

public class GetUserProfileTest : CashFlowClassFixture
{
    private const string METHOD = "api/user";
    private readonly string _token;
    private readonly string _userName;
    private readonly string _userEmail;
    public GetUserProfileTest(CustomWebApplicationFactory webApplicationFactory) : base(webApplicationFactory)
    {
        _token = webApplicationFactory.User_Team_Member.GetToken();
        _userName = webApplicationFactory.User_Team_Member.GetName();
        _userEmail = webApplicationFactory.User_Team_Member.GetEmail();

    }

    [Fact]
    public async Task Success()
    {
        var result = await DoGet(requestUri: METHOD, token: _token);

        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var response = await JsonDocument.ParseAsync(await result.Content.ReadAsStreamAsync());

        response.RootElement.GetProperty("name").GetString().Should().Be(_userName);
        response.RootElement.GetProperty("email").GetString().Should().Be(_userEmail);
    }
}
