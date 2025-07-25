namespace CashFlow.Application.UseCases.Expenses.Reports.Excel;
public interface IGenerateExpensesReportExcelUseCase
{
    Task<byte[]> Execute(DateTime month);
}
