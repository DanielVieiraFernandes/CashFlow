using CashFlow.Infraestructure.DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace WebApi.Test;

/// <summary>
/// Servidor web de teste personalizado para a aplicação ASP.NET Core.<br/>
/// <b>Destinado a testes de integração</b>.<br/>
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        //****************************************************************
        // Mudamos o ambiente para "Test"
        // Assim podemos configurar serviços específicos para testes
        //****************************************************************
        builder.UseEnvironment("Test")
            .ConfigureServices(services =>
            {
                //****************************************************************
                // Registra os serviços internos necessários para o funcionamento
                // do provedor 'In-Memory' e compila um novo ServiceProvider.
                // Este provider será injetado nas opções do DbContext para
                // garantir que o EF utilize a implementação em memória
                // em vez dos serviços padrão de bancos relacionais.
                //****************************************************************
                var provider = services.AddEntityFrameworkInMemoryDatabase()
                    .BuildServiceProvider();

                services.AddDbContext<CashFlowDbContext>(config =>
                {
                    //************************************************************
                    // Configuramos o DbContext para usar um banco de dados
                    // em memória durante os testes de integração.
                    //************************************************************
                    config.UseInMemoryDatabase("InMemoryDbForTesting");
                    config.UseInternalServiceProvider(provider);
                });
            });
    }
}
