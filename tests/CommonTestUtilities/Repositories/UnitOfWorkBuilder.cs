using CashFlow.Domain.Repositories;
using Moq;

namespace CommonTestUtilities.Repositories;
public class UnitOfWorkBuilder
{
    /// <summary>
    /// Devole uma implementação fake da interface IUnitOfWork
    /// </summary>
    /// <returns></returns>
    public static IUnitOfWork Build()
    {
        var mock = new Mock<IUnitOfWork>();

        return mock.Object;
    }
}
