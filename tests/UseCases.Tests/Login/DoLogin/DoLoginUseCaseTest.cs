using CashFlow.Application.UseCases.Login.DoLogin;
using CashFlow.Communication.Requests;
using CashFlow.Domain.Entities;
using CashFlow.Exception;
using CashFlow.Exception.ExceptionsBase;
using CommonTestUtilities.Cryptography;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Token;
using FluentAssertions;

namespace UseCases.Tests.Login.DoLogin;
public class DoLoginUseCaseTest
{

    /*
     * Testes de unidade para o caso de uso de login.
     * -*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
     * - Testar o cenário de sucesso no login.
     * - Testar o cenário de erro quando o usuário não é encontrado.
     * - Testar o cenário de erro quando a senha não confere.
     * -*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
     */

    [Fact]
    public async Task Success()
    {
        var user = UserBuilder.Build();

        RequestLoginJson request = RequestLoginJsonBuilder.Build();
        request.Email = user.Email;
        request.Password = user.Password;

        var useCase = CreateUseCase(user, request.Password);

        var result = await useCase.Execute(request);

        result.Should().NotBeNull();
        result.Name.Should().Be(user.Name);
        result.Token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Error_User_Not_Found()
    {
        var user = UserBuilder.Build();
        RequestLoginJson request = RequestLoginJsonBuilder.Build();

        //*******************************************************************
        // Mantendo os dois e-mails diferentes, garantimos que o
        // usuário não será encontrado e assim testamos o cenário de erro.
        //*******************************************************************

        user.Email = "userexist@gmail.com";
        request.Email = "usernotexist@gmail.com";

        var useCase = CreateUseCase(user, request.Password);

        var act = async Task () => await useCase.Execute(request);

        var result = await act.Should().ThrowAsync<InvalidLoginException>();

        result.Where(ex => ex.GetErrors().Count == 1 &&
            ex.GetErrors().Contains(ResourceErrorMessages.EMAIL_OR_PASSWORD_INVALID)
            );
    }

    [Fact]
    public async Task Error_Password_Not_Match()
    {
        var user = UserBuilder.Build();
        RequestLoginJson request = RequestLoginJsonBuilder.Build();

        request.Email = user.Email;

        var useCase = CreateUseCase(user);

        var act = async Task () => await useCase.Execute(request);

        var result = await act.Should().ThrowAsync<InvalidLoginException>();

        result.Where(ex => ex.GetErrors().Count == 1 &&
            ex.GetErrors().Contains(ResourceErrorMessages.EMAIL_OR_PASSWORD_INVALID)
            );
    }

    private DoLoginUseCase CreateUseCase(User user, string? password = null)
    {
        var passwordEncrypter = new PasswordEncrypterBuilder().Verify(password).Build();
        var tokenGenerator = JwtTokenGeneratorBuilder.Build();
        var readRepository = new UserReadOnlyRepositoryBuilder().GetUserByEmail(user).Build();

        return new DoLoginUseCase(passwordEncrypter, tokenGenerator, readRepository);
    }
}
