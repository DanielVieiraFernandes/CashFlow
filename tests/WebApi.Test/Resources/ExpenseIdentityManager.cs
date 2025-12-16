using CashFlow.Domain.Entities;

namespace WebApi.Test.Resources;

/// <summary>
/// Classe auxiliar para gerenciar a identidade de uma despesa durante os testes.
/// </summary>
public class ExpenseIdentityManager
{
    private readonly Expense _expense;
    public ExpenseIdentityManager(Expense expense)
    {
        _expense = expense;
    }
    public long GetId() => _expense.Id;

    public DateTime GetDate() => _expense.Date;
}
