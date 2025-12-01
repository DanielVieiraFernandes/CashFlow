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

    public async Task<bool> Delete(long id)
    {
        var result = await _dbContext.Expenses.FirstOrDefaultAsync(e => e.Id == id);

        if (result is null) return false;

        _dbContext.Expenses.Remove(result);

        return true;
    }

    public async Task<List<Expense>> GetAll()
    {
        return await _dbContext.Expenses.AsNoTracking().ToListAsync(); // O AsNoTracking faz com que o Entity Framework
                                                                       // não precise ficar monitorando as entidades, ele não salva na memória

    }

    async Task<Expense?> IExpensesReadOnlyRepository.GetById(long id)
    {
        return await _dbContext.Expenses.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
    }

    async Task<Expense?> IExpensesUpdateOnlyRepository.GetById(User user,long id)
    {
        return await _dbContext.Expenses.FirstOrDefaultAsync(e => e.Id == id && e.UserId == user.Id);
    }

    public void Update(Expense expense)
    {
        _dbContext.Expenses.Update(expense);
    }

    public async Task<List<Expense>> FilterByMonth(DateOnly date)
    {
        var startDate = new DateTime(year: date.Year, month: date.Month, day: 1).Date;

        var daysInMonth = DateTime.DaysInMonth(year: date.Year, month: date.Month); // retorna a quantidade de dias
                                                                                    // no mês 
        var endDate = new DateTime(year: date.Year, month: date.Month, day: daysInMonth);

        return await _dbContext
            .Expenses
            .AsNoTracking()
            .Where(expense => expense.Date >= startDate && expense.Date <= endDate)
            .OrderByDescending(expense => expense.Date) // Da menor data para a maior
            .ThenBy(expense => expense.Title) // Ordenação por título
            .ToListAsync();
    }
}
