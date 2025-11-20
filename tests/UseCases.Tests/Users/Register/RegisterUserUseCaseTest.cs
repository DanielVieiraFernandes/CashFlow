using CashFlow.Application.UseCases.Users.Register;
using CashFlow.Communication.Requests;
using CashFlow.Exception;
using CashFlow.Exception.ExceptionsBase;
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
    /*
     * -*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-
     * Necessitamos de testes específicos para cobrir cada cenário possível.
     * Para o exemplo do registro de usuário, temos:
     * - Sucesso no registro
     * - Falha no registro por e-mail já cadastrado
     * - Falha no registro por dados inválidos
     * -*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-
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

    [Fact]
    public async Task Error_Name_Empty()
    {
        RequestRegisterUserJson request = RequestRegisterUserJsonBuilder.Build();
        request.Name = string.Empty;

        RegisterUserUseCase useCase = CreateUseCase();

        // Forma simplificada de testar exceções em métodos assíncronos

        var act = async Task () => await useCase.Execute(request);

        //******************************************************************
        // Eu espero que quando o método for executado,uma exceção do tipo
        // 'ErrorOnValidationException' seja lançada com a mensagem
        // específica de nome vazio
        //******************************************************************

        var result = await act.Should().ThrowAsync<ErrorOnValidationException>();

        result.Where(ex => ex.GetErrors().Count == 1 &&
            ex.GetErrors().Contains(ResourceErrorMessages.NAME_EMPTY)
            );
    }

    [Fact]
    public async Task Error_Email_Already_Exist()
    {
        RequestRegisterUserJson request = RequestRegisterUserJsonBuilder.Build();

        RegisterUserUseCase useCase = CreateUseCase(request.Email);

        var act = async Task () => await useCase.Execute(request);

        var result = await act.Should().ThrowAsync<ErrorOnValidationException>();

        result.Where(ex => ex.GetErrors().Count == 1 &&
            ex.GetErrors().Contains(ResourceErrorMessages.EMAIL_ALREADY_REGISTERED)
            );
    }


    private RegisterUserUseCase CreateUseCase(string? email = null)
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
        var passwordEncrypter = new PasswordEncrypterBuilder().Build();
        var jwtTokenGenerator = JwtTokenGeneratorBuilder.Build();
        var readRepository = new UserReadOnlyRepositoryBuilder();

        if (!string.IsNullOrWhiteSpace(email))
        {
            readRepository.ExistActiveUserWithEmail(email);
        }

        return new RegisterUserUseCase(mapper, passwordEncrypter, readRepository.Build(), writeRepository, unitOfWork, jwtTokenGenerator);
    }
}
