using CashFlow.Domain.Entities;
using CashFlow.Domain.Enums;
using CashFlow.Domain.Security.Cryptography;
using CashFlow.Domain.Security.Tokens;
using CashFlow.Infraestructure.DataAccess;
using CommonTestUtilities.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebApi.Test.Resources;

namespace WebApi.Test;

/// <summary>
/// Servidor web de teste personalizado para a aplicação ASP.NET Core.<br/>
/// <b>Destinado a testes de integração</b>.<br/>
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    //*************************************************************************
    // Estamos deixando os atributos de entidades como privados para
    // impedir com que outra classe externa modifique diretamente os dados
    //*************************************************************************
    public ExpenseIdentityManager Expense_MemberTeam { get; private set; } = default!;
    public ExpenseIdentityManager Expense_Admin { get; private set; } = default!;

    //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-
    // Um usuário com permissão de membro do time
    //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-
    public UserIdentityManager User_Team_Member { get; private set; } = default!;

    //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-
    // Um usuário com permissão de administrador
    //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-
    public UserIdentityManager User_Admin { get; private set; } = default!;

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
                var accessTokenGenerator = scope.ServiceProvider.GetRequiredService<IAccessTokenGenerator>();

                StartDatabase(dbContext, passwordEncrypter, accessTokenGenerator);
            });
    }

    private void StartDatabase(
        CashFlowDbContext dbContext,
        IPasswordEncrypter passwordEncrypter,
        IAccessTokenGenerator accessTokenGenerator)
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

        var userTeamMember = AddUserTeamMember(dbContext, passwordEncrypter, accessTokenGenerator);
        var expenseTeamMember = AddExpenses(dbContext, userTeamMember, expenseId: 1, lastTagId: 0);
        Expense_MemberTeam = new(expenseTeamMember.Item1);

        var userAdmin = AddUserAdmin(dbContext, passwordEncrypter, accessTokenGenerator);
        var expenseAdmin = AddExpenses(dbContext, userAdmin, expenseId: 2, lastTagId: expenseTeamMember.Item2);
        Expense_Admin = new(expenseAdmin.Item1);

        dbContext.SaveChanges();
    }

    private User AddUserTeamMember(
        CashFlowDbContext dbContext,
        IPasswordEncrypter passwordEncrypter,
        IAccessTokenGenerator accessTokenGenerator)
    {
        var user = UserBuilder.Build();

        // Garanto que o Id seja sempre 1 para este usuário
        user.Id = 1;

        var password = user.Password;

        user.Password = passwordEncrypter.Encrypt(user.Password);

        dbContext.Users.Add(user);

        //*******************************************************************
        // Gera um token para o usuário criado no banco de dados em memória.
        // Útil para testes de rotas com autenticação.
        //*******************************************************************
        var token = accessTokenGenerator.Generate(user);

        User_Team_Member = new(user, password, token);

        return user;
    }

    private User AddUserAdmin(
        CashFlowDbContext dbContext,
        IPasswordEncrypter passwordEncrypter,
        IAccessTokenGenerator accessTokenGenerator)
    {
        // Sobrescrevo o parâmetro com o valor da permissão de administrador
        var user = UserBuilder.Build(Roles.ADMIN);

        // Garanto que o Id seja sempre 2 para este usuário
        user.Id = 2;

        var password = user.Password;

        user.Password = passwordEncrypter.Encrypt(user.Password);

        dbContext.Users.Add(user);

        //*******************************************************************
        // Gera um token para o usuário criado no banco de dados em memória.
        // Útil para testes de rotas com autenticação.
        //*******************************************************************
        var token = accessTokenGenerator.Generate(user);

        User_Admin = new(user, password, token);

        return user;
    }

    private (Expense, long) AddExpenses(CashFlowDbContext dbContext, User user, long expenseId, long lastTagId)
    {
        var expense = ExpenseBuilder.Build(user);

        // Garanto que o Id seja sempre diferente para cada despesa
        expense.Id = expenseId;

        //************************************************************************
        // Garanto que para cada entidade tag, o expenseId seja o correto
        // pois no mock do ExpenseBuilder, as tags possuem um ExpenseId fixo
        //************************************************************************
        foreach (var tag in expense.Tags)
        {
            lastTagId++;

            tag.Id = lastTagId;
            tag.ExpenseId = expense.Id;
        }

        dbContext.Expenses.Add(expense);

        return (expense, lastTagId);
    }
}
