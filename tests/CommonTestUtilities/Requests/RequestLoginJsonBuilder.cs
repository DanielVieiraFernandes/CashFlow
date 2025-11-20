using Bogus;
using CashFlow.Communication.Requests;

namespace CommonTestUtilities.Requests;
public class RequestLoginJsonBuilder
{
    public static RequestLoginJson Build()
    {
        return new Faker<RequestLoginJson>()
            .RuleFor(u => u.Email, faker => faker.Internet.Email())
            .RuleFor(u => u.Password, faker => faker.Internet.Password(prefix: "!Aa1"));
    }
}
