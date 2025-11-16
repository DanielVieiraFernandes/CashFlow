using CashFlow.Domain.Security.Cryptography;
using Moq;

namespace CommonTestUtilities.Cryptography;
public class PasswordEncrypterBuilder
{
    public static IPasswordEncrypter Build()
    {
        var mock = new Mock<IPasswordEncrypter>();

        // Configura o mock para retornar um hash fixo
        // Não interessa qual o valor da senha fornecida
        mock.Setup(passwordEncrypter => passwordEncrypter.Encrypt(It.IsAny<string>())).Returns("hashed_password");

        return mock.Object;
    }
}
