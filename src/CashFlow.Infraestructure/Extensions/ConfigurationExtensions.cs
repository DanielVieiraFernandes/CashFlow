using Microsoft.Extensions.Configuration;

namespace CashFlow.Infraestructure.Extensions;
public static class ConfigurationExtensions
{
    public static bool IsTestEnvironment(this IConfiguration configuration)
    {
        // Retorna um valor booleano indicando se o ambiente de teste em memória está ativo
        return configuration.GetValue<bool>("InMemoryTest");
    }
}
