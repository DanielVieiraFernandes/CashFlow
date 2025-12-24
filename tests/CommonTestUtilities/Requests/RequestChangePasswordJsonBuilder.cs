using Bogus;
using CashFlow.Communication.Requests;

namespace CommonTestUtilities.Requests;

public class RequestChangePasswordJsonBuilder
{
    public static RequestChangePasswordJson Build()
    {
        return new Faker<RequestChangePasswordJson>()
            .RuleFor(user => user.Password, faker => faker.Internet.Password())
            //*******************************************
            // Garanto que a nova senha esteja de acordo
            // com os requisitos mínimos de complexidade
            //*******************************************
            .RuleFor(user => user.NewPassword, faker => faker.Internet.Password(prefix: "!Aa1"));
    }
}
