using CashFlow.Domain.Security.Cryptography;
using Moq;

namespace CommonTestUtilities.Cryptography;
public class PasswordEncrypterBuilder
{
    private readonly Mock<IPasswordEncrypter> _mock;

    public PasswordEncrypterBuilder()
    {
        _mock = new Mock<IPasswordEncrypter>();

        // Configura o mock para retornar um hash fixo
        // Não interessa qual o valor da senha fornecida
        _mock.Setup(passwordEncrypter => passwordEncrypter.Encrypt(It.IsAny<string>())).Returns("!Aa1ahsuh");
    }
    public IPasswordEncrypter Build() => _mock.Object;

    public PasswordEncrypterBuilder Verify(string? password = null)
    {
        if (!string.IsNullOrWhiteSpace(password))
        {
            _mock.Setup(passwordEncrypter => passwordEncrypter.Verify(password, It.IsAny<string>())).Returns(true);
        }

        return this;
    }
}
