using Bogus;
using CashFlow.Domain.Entities;
using CashFlow.Domain.Enums;
using CommonTestUtilities.Cryptography;

namespace CommonTestUtilities.Entities;

public class UserBuilder
{
    public static User Build(string role = Roles.TEAM_MEMBER)
    {
        var passwordEncrypter = new PasswordEncrypterBuilder().Build();

        var user = new Faker<User>()
            .RuleFor(u => u.Name, f => f.Person.FirstName)
            .RuleFor(u => u.Email, (f, user) => f.Internet.Email(user.Name))
            .RuleFor(u => u.Password, (_, user) => passwordEncrypter.Encrypt(user.Password))
            .RuleFor(u => u.Role, _ => role)
            .RuleFor(u => u.UserIdentifier, _ => Guid.NewGuid());

        return user;
    }
}
