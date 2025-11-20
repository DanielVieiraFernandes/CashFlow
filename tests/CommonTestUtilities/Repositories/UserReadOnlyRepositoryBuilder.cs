using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories.User;
using Moq;

namespace CommonTestUtilities.Repositories;
public class UserReadOnlyRepositoryBuilder
{
    private readonly Mock<IUserReadOnlyRepository> _repository;

    public UserReadOnlyRepositoryBuilder()
    {
        _repository = new Mock<IUserReadOnlyRepository>();
    }

    public void ExistActiveUserWithEmail(string email)
    {
        _repository
            .Setup(userReadOnly => userReadOnly.ExistActiveUserWithEmail(email))
            .ReturnsAsync(true);
    }

    public UserReadOnlyRepositoryBuilder GetUserByEmail(User user)
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Recebo um usuário e configuro o mock para retornar esse usuário
        // Ao chamar o método GetUserByEmail com o email do usuário fornecido
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        _repository.Setup(userRepository => userRepository.GetUserByEmail(user.Email))
            .ReturnsAsync(user);

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Quando retorno o this, significa que estou devolvendo o próprio objeto
        // Possibilitando o encadeamento de chamadas (method chaining)
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        return this;
    }

    public IUserReadOnlyRepository Build() => _repository.Object;
}
