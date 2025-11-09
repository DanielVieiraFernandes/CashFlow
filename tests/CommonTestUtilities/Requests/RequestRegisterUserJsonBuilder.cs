using Bogus;
using CashFlow.Communication.Requests;

namespace CommonTestUtilities.Requests;
public class RequestRegisterUserJsonBuilder
{
    public static RequestRegisterUserJson Build()
    {
        return new Faker<RequestRegisterUserJson>()
            .RuleFor(f => f.Name, faker => faker.Person.FirstName)
            .RuleFor(f => f.Email, (faker, user) => faker.Internet.Email(user.Name))
            .RuleFor(f => f.Password, faker => faker.Internet.Password(prefix: "!Aa1"));
    }
}
