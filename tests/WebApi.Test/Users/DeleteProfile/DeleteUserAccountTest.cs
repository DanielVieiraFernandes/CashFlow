using CashFlow.Communication.Requests;
using FluentAssertions;
using System.Net;

namespace WebApi.Test.Users.DeleteProfile;

public class DeleteUserAccountTest : CashFlowClassFixture
{
    private const string METHOD = "api/user";
    private readonly string _token;
    private readonly string _email;
    private readonly string _password;
    public DeleteUserAccountTest(CustomWebApplicationFactory webApplicationFactory) : base(webApplicationFactory)
    {
        _token = webApplicationFactory.User_Team_Member.GetToken();
        _email = webApplicationFactory.User_Team_Member.GetEmail();
        _password = webApplicationFactory.User_Team_Member.GetPassword();
    }

    [Fact]
    public async Task Success()
    {
        var result = await DoDelete(requestUri: METHOD, token: _token);

        result.StatusCode.Should().Be(HttpStatusCode.NoContent);

        RequestLoginJson request = new()
        {
            Email = _email,
            Password = _password
        };

        result = await DoPost(requestUri: $"api/login", request, token: _token);

        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
