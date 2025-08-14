
using CashFlow.Domain.Repositories.Expenses;

namespace CashFlow.Application.UseCases.Expenses.Reports.Pdf;
public class GenerateExpenseReportPdfUseCase : IGenerateExpenseReportPdfUseCase
{
    private const string CURRENCY_SYMBOL = "$";
    private readonly IExpensesReadOnlyRepository _repository;
    public GenerateExpenseReportPdfUseCase(IExpensesReadOnlyRepository repository)
    {
        _repository = repository;
    }
    public async Task<byte[]> Execute(DateOnly month)
    {
        var expenses = await _repository.FilterByMonth(month);
        if (expenses.Count == 0)
        {
            return [];
        }

        return [];
    }
}
