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
        };

        //new Faker<RequestExpenseJson>() // tem essa alternativa também
        //    .RuleFor(f => f.Amount, faker => faker.Random.Decimal());

        return request;
    }
}
