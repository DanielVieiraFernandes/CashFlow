using CashFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Infraestructure.DataAccess;
public class CashFlowDbContext : DbContext
{
    public CashFlowDbContext(DbContextOptions options) : base(options) // estou repassando para o construtor
                                                                       // da classe base as options
                                                                       // que estou recebendo
    {

    }
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<User> Users { get; set; }
}