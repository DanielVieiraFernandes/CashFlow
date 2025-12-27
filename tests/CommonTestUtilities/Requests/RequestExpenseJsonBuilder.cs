using Bogus;
using CashFlow.Communication.Enums;
using CashFlow.Communication.Requests;

namespace CommonTestUtilities.Requests;

public class RequestExpenseJsonBuilder
{
    public static RequestExpenseJson Build()
    {
        var faker = new Faker();
        var request = new RequestExpenseJson
        {
            Title = faker.Commerce.ProductName(),
            Date = faker.Date.Past(),
            PaymentType = faker.PickRandom<PaymentType>(),
            Description = faker.Commerce.ProductDescription(),
            Amount = faker.Random.Decimal(min: 1, max: 1000),
            Tags = faker.Make(2, () => faker.PickRandom<Tag>()),
        };

        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
        // Exemplo alternativo usando object initializer
        //new Faker<RequestExpenseJson>() 
        //    .RuleFor(f => f.Amount, faker => faker.Random.Decimal());
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*

        return request;
    }
}
