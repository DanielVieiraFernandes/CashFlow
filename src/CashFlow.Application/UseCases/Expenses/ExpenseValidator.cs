using CashFlow.Communication.Requests;
using CashFlow.Exception;
using FluentValidation;

namespace CashFlow.Application.UseCases.Expenses;

public class ExpenseValidator : AbstractValidator<RequestExpenseJson>
{

    public ExpenseValidator()
    {
        RuleFor(expense => expense.Title).NotEmpty().WithMessage(ResourceErrorMessages.TITLE_REQUIRED);
        RuleFor(expense => expense.Amount).GreaterThan(0).WithMessage
            (ResourceErrorMessages.AMOUNT_MUST_BE_GREATHER_THAN);
        RuleFor(expense => expense.Date).LessThanOrEqualTo(DateTime.UtcNow).
            WithMessage(ResourceErrorMessages.EXPENSES_CANNOT_FOR_THE_FUTURE);
        RuleFor(expense => expense.PaymentType).IsInEnum().WithMessage(ResourceErrorMessages.PAYMENT_TYPE_INVALID);

        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
        // Verifica se todas as tags são válidas.
        // Se os valores estão dentro do enum Tag
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
        RuleFor(expense => expense.Tags).ForEach(tag =>
        {
            tag.IsInEnum().WithMessage(ResourceErrorMessages.TAG_TYPE_NOT_SUPPORTED);
        });
    }

}
