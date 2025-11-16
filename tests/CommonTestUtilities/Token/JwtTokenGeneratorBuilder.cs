using CashFlow.Domain.Entities;
using CashFlow.Domain.Security.Tokens;
using Moq;

namespace CommonTestUtilities.Token;
public class JwtTokenGeneratorBuilder
{
    /// <summary>
    /// Retorna um mock de IAccessTokenGenerator
    /// </summary>
    /// <returns></returns>
    public static IAccessTokenGenerator Build()
    {
        var mock = new Mock<IAccessTokenGenerator>();

        // Configura o mock para retornar um token fixo
        // o It.IsAny<User>() indica que o método Generate pode ser chamado e ignora o parâmetro User passado
        // isso indica que não estamos preocupados com o valor específico do User passado para o método
        // e queremos sempre retornar "token" independentemente do User fornecido
        mock.Setup(accessTokenGenerator => accessTokenGenerator.Generate(It.IsAny<User>())).Returns("token");

        return mock.Object;
    }
}
