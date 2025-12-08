using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories.Expenses;
using Moq;

namespace CommonTestUtilities.Repositories;

public class ExpensesReadOnlyRepositoryBuilder
{
    private readonly Mock<IExpensesReadOnlyRepository> _repository;

    public ExpensesReadOnlyRepositoryBuilder()
    {
        _repository = new Mock<IExpensesReadOnlyRepository>();
    }

    public ExpensesReadOnlyRepositoryBuilder GetAll(User user, List<Expense> expenses)
    {
        //***************************************************************************
        // Define que ao chamar o método GetAll com o usuário especificado,
        // o repositório irá retornar a lista de despesas fornecida para o mock.
        //***************************************************************************
        _repository.Setup(repository => repository.GetAll(user)).ReturnsAsync(expenses);

        return this;
    }

    public ExpensesReadOnlyRepositoryBuilder GetById(User user, Expense? expense)
    {
        //***************************************************************************************
        // Define que ao chamar o método GetById com o usuário e o ID da despesa especificados,
        // o repositório irá retornar a despesa fornecida para o mock.
        //***************************************************************************************
        if (expense is not null)
            _repository.Setup(repository => repository.GetById(user, expense.Id)).ReturnsAsync(expense);

        return this;
    }

    public IExpensesReadOnlyRepository Build() => _repository.Object;
}
