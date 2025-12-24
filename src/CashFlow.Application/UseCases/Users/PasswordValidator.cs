using CashFlow.Exception;
using FluentValidation;
using FluentValidation.Validators;
using System.Text.RegularExpressions;

namespace CashFlow.Application.UseCases.Users;

public class PasswordValidator<T> : PropertyValidator<T, string>
{
    private const string ERROR_MESSAGE_KEY = "ErrorMessage";
    public override string Name => "PasswordValidator";

    protected override string GetDefaultMessageTemplate(string errorCode)
    {
        // Precisamos ensinar como esse validator vai devolver as mensagens de erro

        return $"{{{ERROR_MESSAGE_KEY}}}";
    }


    public override bool IsValid(ValidationContext<T> context, string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            context.MessageFormatter.
                AppendArgument(ERROR_MESSAGE_KEY, ResourceErrorMessages.INVALID_PASSWORD);

            return false;
        }

        if (password.Length < 8)
        {
            context.MessageFormatter.
                AppendArgument(ERROR_MESSAGE_KEY, ResourceErrorMessages.INVALID_PASSWORD);

            return false;
        }

        if (Regex.IsMatch(password, @"[A-Z]+") == false)
        {
            context.MessageFormatter.
                AppendArgument(ERROR_MESSAGE_KEY, ResourceErrorMessages.INVALID_PASSWORD);

            return false;
        }

        if (Regex.IsMatch(password, @"[a-z]+") == false)
        {
            context.MessageFormatter.
                AppendArgument(ERROR_MESSAGE_KEY, ResourceErrorMessages.INVALID_PASSWORD);

            return false;
        }

        if (Regex.IsMatch(password, @"[0-9]+") == false)
        {
            context.MessageFormatter.
                AppendArgument(ERROR_MESSAGE_KEY, ResourceErrorMessages.INVALID_PASSWORD);

            return false;
        }

        if (Regex.IsMatch(password, @"[\!\?\*\.]+") == false)
        {
            context.MessageFormatter.
                AppendArgument(ERROR_MESSAGE_KEY, ResourceErrorMessages.INVALID_PASSWORD);

            return false;
        }

        return true;
    }
}
