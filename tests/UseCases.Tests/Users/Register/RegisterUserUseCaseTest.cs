using CashFlow.Application.UseCases.Users.Register;
using CashFlow.Communication.Requests;
using CommonTestUtilities.Cryptography;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Token;
using FluentAssertions;

namespace UseCases.Tests.Users.Register;
public class RegisterUserUseCaseTest
{
    /*
     * Sobre os Testes de Unidade:
     * -*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
     * Como queremos testar apenas um método, uma unidade da aplicação,
     * para realizar a injeção de dependências, iremos utilizar implementações fake
     * (apenas uma real, o automapper, pois precisamos dele para mapear um objeto para outro)
     * pois não estamos preocupados com dependências externas, queremos apenas poder testar se o caso de uso
     * atende ao seu propósito, independente de banco de dados, independente de biblioteca externa.
     * -*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
     */


    [Fact]
    public async Task Success()
    {
        RequestRegisterUserJson request = RequestRegisterUserJsonBuilder.Build();

        RegisterUserUseCase useCase = CreateUseCase();

        var result = await useCase.Execute(request);

        result.Should().NotBeNull();
        result.Name.Should().Be(request.Name);
        result.Token.Should().NotBeNullOrWhiteSpace();
    }

    private RegisterUserUseCase CreateUseCase()
    {
        // Aqui é uma exceção pois precisamos dele para preencher o objeto, é apenas uma facilidade no código
        var mapper = MapperBuilder.Build();

        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-
        // Criando mocks
        //
        // O que é um mock?
        // Um mock é uma implementação fake de uma determinada interface.
        // Aqui por exemplo, utilizamos um pacote chamado 'Moq'
        // que utiliza uma interface para criar implementações fakes.
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-
        var unitOfWork = UnitOfWorkBuilder.Build();
        var writeRepository = UserWriteOnlyRepositoryBuilder.Build();
        var passwordEncrypter = PasswordEncrypterBuilder.Build();
        var jwtTokenGenerator = JwtTokenGeneratorBuilder.Build();
        var readRepository = new UserReadOnlyRepositoryBuilder().Build();

        return new RegisterUserUseCase(mapper, passwordEncrypter, readRepository, writeRepository, unitOfWork, jwtTokenGenerator);
    }
}
