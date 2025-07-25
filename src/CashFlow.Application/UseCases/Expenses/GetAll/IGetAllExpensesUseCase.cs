using CashFlow.Communication.Responses;

namespace CashFlow.Application.UseCases.Expenses.GetAll;
public interface IGetAllExpensesUseCase
{
    Task<ResponseExpensesJson> Execute();
}
