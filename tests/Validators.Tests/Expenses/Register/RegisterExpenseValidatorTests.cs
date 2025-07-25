using CashFlow.Application.UseCases.Expenses;
using CashFlow.Communication.Enums;
using CashFlow.Exception;
using CommonTestUtilities.Requests;
using FluentAssertions;

namespace Validators.Tests.Expenses.Register;
public class RegisterExpenseValidatorTests
{

    [Fact]
    public void Success()
    {
        // Arrange // parte onde a gente configura todas as instâncias para executar o teste unitário

        var validator = new ExpenseValidator();

        var request = RequestExpenseJsonBuilder.Build();

        // Act // executar o método que a gente quer testar

        var result = validator.Validate(request);

        // Assert // Eu espero um resultado, no caso dessa função, queremos que seja um resultado verdadeiro

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("             ")]
    [InlineData(null)]
    public void Error_Title_Empty(string title)
    {
        // Arrange // parte onde a gente configura todas as instâncias para executar o teste unitário

        var validator = new ExpenseValidator();
        var request = RequestExpenseJsonBuilder.Build();
        request.Title = string.Empty;

        // Act // executar o método que a gente quer testar

        var result = validator.Validate(request);

        // Assert // Eu espero um resultado, no caso dessa função, queremos que seja um resultado verdadeiro

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .And.Contain(e => e.ErrorMessage.Equals(ResourceErrorMessages.TITLE_REQUIRED));
    }

    [Fact]
    public void Error_Date_Future()
    {
        // Arrange // parte onde a gente configura todas as instâncias para executar o teste unitário

        var validator = new ExpenseValidator();
        var request = RequestExpenseJsonBuilder.Build();
        request.Date = DateTime.UtcNow.AddDays(1);

        // Act // executar o método que a gente quer testar

        var result = validator.Validate(request);

        // Assert // Eu espero um resultado, no caso dessa função, queremos que seja um resultado verdadeiro

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .And.Contain(e => e.ErrorMessage.Equals(ResourceErrorMessages.EXPENSES_CANNOT_FOR_THE_FUTURE));
    }

    [Fact]
    public void Error_Payment_Type_Invalid()
    {
        // Arrange // parte onde a gente configura todas as instâncias para executar o teste unitário

        var validator = new ExpenseValidator();
        var request = RequestExpenseJsonBuilder.Build();
        request.PaymentType = (PaymentType)700;

        // Act // executar o método que a gente quer testar

        var result = validator.Validate(request);

        // Assert // Eu espero um resultado, no caso dessa função, queremos que seja um resultado verdadeiro

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .And.Contain(e => e.ErrorMessage.Equals(ResourceErrorMessages.PAYMENT_TYPE_INVALID));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-2)]
    public void Error_Amount_Invalid(decimal amount)
    {
        // Arrange // parte onde a gente configura todas as instâncias para executar o teste unitário

        var validator = new ExpenseValidator();
        var request = RequestExpenseJsonBuilder.Build();
        request.Amount = amount;

        // Act // executar o método que a gente quer testar

        var result = validator.Validate(request);

        // Assert // Eu espero um resultado, no caso dessa função, queremos que seja um resultado verdadeiro

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .And.Contain(e => e.ErrorMessage.Equals(ResourceErrorMessages.AMOUNT_MUST_BE_GREATHER_THAN));
    }
}