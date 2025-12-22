using Bogus;
using CashFlow.Communication.Requests;

namespace CommonTestUtilities.Requests;

public class RequestUpdateUserJsonBuilder
{
    public static RequestUpdateUserJson Build()
    {
        return new Faker<RequestUpdateUserJson>()
            .RuleFor(r => r.Name, f => f.Person.FullName)
            .RuleFor(r => r.Email, (f, u) => f.Internet.Email(u.Name));
    }
}
