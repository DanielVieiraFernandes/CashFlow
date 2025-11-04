namespace CashFlow.Domain.Security.Cryptography;
public interface IPasswordEncrypter
{
    string Encrypt(string password);
    bool Verify(string password, string passwordHash);
}
