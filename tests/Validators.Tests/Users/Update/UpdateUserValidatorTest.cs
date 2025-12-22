using CashFlow.Application.UseCases.Users.Update;
using CashFlow.Exception;
using CommonTestUtilities.Requests;
using FluentAssertions;

namespace Validators.Tests.Users.Update;

public class UpdateUserValidatorTest
{
    //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
    // Testes de unidade para o validator
    // de atualização de usuário
    //
    // Cenários:
    // - Sucesso
    // - Erro: Nome vazio
    // - Erro: Email vazio
    // - Erro: Email inválido
    //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*

    [Fact]
    public void Success()
    {
        //-*-*-*-*-*-*-*-*-*-*-*-*
        // Arrange
        //-*-*-*-*-*-*-*-*-*-*-*-*

        // Crio uma instância do validator

        var validator = new UpdateUserValidator();

        // Crio um objeto de request válido
        var request = RequestUpdateUserJsonBuilder.Build();

        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*

        var result = validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("          ")]
    [InlineData(null)]
    public void Error_Name_Empty(string name)
    {
        var validator = new UpdateUserValidator();
        var request = RequestUpdateUserJsonBuilder.Build();
        request.Name = name;

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle().And.Contain(e => e.ErrorMessage.Equals(ResourceErrorMessages.NAME_EMPTY));
    }

    [Theory]
    [InlineData("")]
    [InlineData("          ")]
    [InlineData(null)]
    public void Error_Email_Empty(string email)
    {
        var validator = new UpdateUserValidator();
        var request = RequestUpdateUserJsonBuilder.Build();
        request.Email = email;

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle().And.Contain(e => e.ErrorMessage.Equals(ResourceErrorMessages.EMAIL_EMPTY));
    }

    [Fact]
    public void Error_Email_Invalid()
    {
        var validator = new UpdateUserValidator();
        var request = RequestUpdateUserJsonBuilder.Build();
        request.Email = "daniel.com";

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle().And.Contain(e => e.ErrorMessage.Equals(ResourceErrorMessages.EMAIL_INVALID));
    }
}
