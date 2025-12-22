using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories.User;
using Moq;

namespace CommonTestUtilities.Repositories;

public class UserUpdateOnlyRepositoryBuilder
{
    public static IUserUpdateOnlyRepository Build(User user)
    {
        var mock = new Mock<IUserUpdateOnlyRepository>();

        //**************************************************************************************
        // No próprio método de build, estou passando a entidade user
        // para configurar o Mock para retornar essa entidade quando GetById for chamado
        //**************************************************************************************

        mock.Setup(repo => repo.GetById(user.Id)).ReturnsAsync(user);

        return mock.Object;
    }
}
