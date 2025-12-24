using CashFlow.Communication.Requests;
using CashFlow.Exception;
using CommonTestUtilities.Requests;
using FluentAssertions;
using System.Text.Json;
using WebApi.Test.InlineData;

namespace WebApi.Test.Users.ChangePassword;

public class ChangePasswordTest : CashFlowClassFixture
{
    private const string METHOD = "api/user/change-password";

    private readonly string _token;
    private readonly string _password;
    private readonly string _email;
    public ChangePasswordTest(CustomWebApplicationFactory webApplicationFactory) : base(webApplicationFactory)
    {
        _token = webApplicationFactory.User_Team_Member.GetToken();
        _password = webApplicationFactory.User_Team_Member.GetPassword();
        _email = webApplicationFactory.User_Team_Member.GetEmail();
    }

    [Fact]
    public async Task Success()
    {
        var request = RequestChangePasswordJsonBuilder.Build();

        //****************************************************************
        // Passo a senha atual do usuário para o objeto de request 
        // para que passe na validação de confirmação da senha atual
        //****************************************************************

        request.Password = _password;

        var response = await DoPut(METHOD, request, _token);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);

        //****************************************************************
        // É esperado que após a alteração da senha, que o usuário
        // não possa mais logar com a senha anterior
        //****************************************************************

        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
        // Tenta fazer o login com a senha anterior
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
        var loginRequest = new RequestLoginJson { Email = _email, Password = _password };
        response = await DoPost("api/login", loginRequest);
        //-*-*-*-*-*-*-*-*-
        // Espero um erro
        //-*-*-*-*-*-*-*-*-
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);

        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
        // Tenta fazer o login com a nova senha
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
        loginRequest.Password = request.NewPassword;
        response = await DoPost("api/login", loginRequest);
        //-*-*-*-*-*-*-*-*-*-*-
        // Espero um sucesso
        //-*-*-*-*-*-*-*-*-*-*-
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_Password_Different_Current_Password(string culture)
    {
        var request = RequestChangePasswordJsonBuilder.Build();

        var response = await DoPut(METHOD, request, token: _token, cultureInfo: culture);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errorMessages").EnumerateArray();

        var expectedMessage = ResourceErrorMessages.ResourceManager.GetString("PASSWORD_DIFFERENT_CURRENT_PASSWORD",
            new System.Globalization.CultureInfo(culture));

        errors.Should().HaveCount(1).And.Contain(c => c.GetString()!.Equals(expectedMessage));
    }
}
