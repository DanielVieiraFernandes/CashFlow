using CashFlow.Domain.Entities;
using CashFlow.Domain.Security.Cryptography;
using CashFlow.Domain.Security.Tokens;
using CashFlow.Infraestructure.DataAccess;
using CommonTestUtilities.Entities;
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
    private Expense _expense;
    private User _user;
    private string _password;
    private string _token;
    public string GetEmail() => _user.Email;
    public string GetName() => _user.Name;
    public string GetPassword() => _password;
    public string GetToken() => _token;
    public long GetExpenseId() => _expense.Id;
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

                //*******************************************************************
                // Com o provider que criamos, podemos criar um escopo, simulando
                // o escopo de uma requisição, conseguimos recuperar do serviço
                // de injeção de dependências o DbContext configurado para usar
                // o banco em memória.
                //*******************************************************************
                var scope = services.BuildServiceProvider().CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<CashFlowDbContext>();
                var passwordEncrypter = scope.ServiceProvider.GetRequiredService<IPasswordEncrypter>();
                var tokenGenerator = scope.ServiceProvider.GetRequiredService<IAccessTokenGenerator>();

                StartDatabase(dbContext, passwordEncrypter);

                //*******************************************************************
                // Gera um token para o usuário criado no banco de dados em memória.
                // Útil para testes de rotas com autenticação.
                //*******************************************************************
                _token = tokenGenerator.Generate(_user);
            });
    }

    private void StartDatabase(CashFlowDbContext dbContext,
        IPasswordEncrypter passwordEncrypter)
    {
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Popula o banco de dados em memória com dados iniciais:
        // - Usuários
        // - Despesas
        // -*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
        // Essas operações ocorrem no momento em que a classe
        // é instanciada, garantindo que cada teste tenha
        // um estado inicial consistente e previsível.
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++

        AddUsers(dbContext, passwordEncrypter);
        AddExpenses(dbContext);

        dbContext.SaveChanges();
    }

    private void AddUsers(CashFlowDbContext dbContext, IPasswordEncrypter passwordEncrypter)
    {
        _user = UserBuilder.Build();
        _password = _user.Password;

        _user.Password = passwordEncrypter.Encrypt(_user.Password);

        dbContext.Users.Add(_user);
    }

    private void AddExpenses(CashFlowDbContext dbContext)
    {
        _expense = ExpenseBuilder.Build(_user);
        dbContext.Expenses.Add(_expense);
    }
}
