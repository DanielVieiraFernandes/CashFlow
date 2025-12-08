using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories.Expenses;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Infraestructure.DataAccess.Repositories;

internal class ExpensesRepository : IExpensesReadOnlyRepository, IExpensesWriteOnlyRepository,
    IExpensesUpdateOnlyRepository
{
    private readonly CashFlowDbContext _dbContext;
    public ExpensesRepository(CashFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task Add(Expense expense)
    {
        await _dbContext.Expenses.AddAsync(expense);
    }

    public async Task Delete(long id)
    {
        var result = await _dbContext.Expenses.FirstAsync(e => e.Id == id);

        _dbContext.Expenses.Remove(result);
    }

    public async Task<List<Expense>> GetAll(User user)
    {
        return await _dbContext.Expenses.AsNoTracking().Where(e => e.UserId == user.Id).ToListAsync(); // O AsNoTracking faz com que o Entity Framework
                                                                                                       // não precise ficar monitorando as entidades, ele não salva na memória
    }

    async Task<Expense?> IExpensesReadOnlyRepository.GetById(User user, long id)
    {
        return await _dbContext.Expenses.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id && e.UserId == user.Id);
    }

    async Task<Expense?> IExpensesUpdateOnlyRepository.GetById(User user, long id)
    {
        return await _dbContext.Expenses.FirstOrDefaultAsync(e => e.Id == id && e.UserId == user.Id);
    }

    public void Update(Expense expense)
    {
        _dbContext.Expenses.Update(expense);
    }

    public async Task<List<Expense>> FilterByMonth(User user, DateOnly date)
    {
        var startDate = new DateTime(year: date.Year, month: date.Month, day: 1).Date;

        var daysInMonth = DateTime.DaysInMonth(year: date.Year, month: date.Month); // retorna a quantidade de dias
                                                                                    // no mês 
        var endDate = new DateTime(year: date.Year, month: date.Month, day: daysInMonth);

        return await _dbContext
            .Expenses
            .AsNoTracking()
            .Where(expense => expense.UserId == user.Id && expense.Date >= startDate && expense.Date <= endDate)
            .OrderByDescending(expense => expense.Date) // Da menor data para a maior
            .ThenBy(expense => expense.Title) // Ordenação por título
            .ToListAsync();
    }
}
