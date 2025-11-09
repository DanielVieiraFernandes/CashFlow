using CashFlow.Application.UseCases.Users.Register;
using CashFlow.Exception;
using CommonTestUtilities.Requests;
using FluentAssertions;

namespace Validators.Tests.Users.Register;
public class RegisterUserValidatorTest
{

    [Fact]
    public void Success()
    {
        //Arrange
        var request = RequestRegisterUserJsonBuilder.Build();
        var validator = new RegisterUserValidator();

        //Act
        var result = validator.Validate(request);

        //Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("          ")]
    [InlineData(null)]
    public void Error_Name_Empty(string name)
    {
        //Arrange
        var request = RequestRegisterUserJsonBuilder.Build();
        var validator = new RegisterUserValidator();

        request.Name = name;

        //Act
        var result = validator.Validate(request);

        //Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle().And.Contain(e => e.ErrorMessage == ResourceErrorMessages.NAME_EMPTY);
    }

    [Theory]
    [InlineData("")]
    [InlineData("          ")]
    [InlineData(null)]
    public void Error_Email_Empty(string email)
    {
        //Arrange
        var request = RequestRegisterUserJsonBuilder.Build();
        var validator = new RegisterUserValidator();

        request.Email = email;

        //Act
        var result = validator.Validate(request);

        //Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle().And.Contain(e => e.ErrorMessage == ResourceErrorMessages.EMAIL_EMPTY);
    }

    [Fact]
    public void Error_Email_Invalid()
    {
        //Arrange
        var request = RequestRegisterUserJsonBuilder.Build();
        var validator = new RegisterUserValidator();

        request.Email = "daniel.com";

        //Act
        var result = validator.Validate(request);

        //Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle().And.Contain(e => e.ErrorMessage == ResourceErrorMessages.EMAIL_INVALID);
    }

    [Fact]
    public void Error_Password_Empty()
    {
        //Arrange
        var request = RequestRegisterUserJsonBuilder.Build();
        var validator = new RegisterUserValidator();

        request.Password = string.Empty;

        //Act
        var result = validator.Validate(request);

        //Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle().And.Contain(e => e.ErrorMessage == ResourceErrorMessages.INVALID_PASSWORD);
    }
}
