//-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
// Comando que cria uma migration: 
// 
// dotnet ef migrations add TagsIntoExpenses --project.\CashFlow.Infraestructure\
// --startup-project.\CashFlow.Api\
//
// O comando deve ser executado na pasta src do projeto.
//-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+


using CashFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Infraestructure.DataAccess;

public class CashFlowDbContext : DbContext
{

    //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
    // estou repassando para o construtor
    // da classe base as options
    // que estou recebendo
    //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
    public CashFlowDbContext(DbContextOptions options) : base(options)
    {

    }
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //*************************************************
        // Sobrescrevo o método para que a migration
        // crie a tabela no plural
        //*************************************************
        modelBuilder.Entity<Tag>().ToTable("Tags");
    }
}