namespace CashFlow.Application.UseCases.Expenses.Reports.Pdf;
public interface IGenerateExpenseReportPdfUseCase
{
    Task<byte[]> Execute(DateOnly month);
}
