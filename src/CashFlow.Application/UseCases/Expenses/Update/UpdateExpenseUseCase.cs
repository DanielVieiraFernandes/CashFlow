using AutoMapper;
using CashFlow.Communication.Requests;
using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.Expenses;
using CashFlow.Domain.Services.LoggedUser;
using CashFlow.Exception;
using CashFlow.Exception.ExceptionsBase;

namespace CashFlow.Application.UseCases.Expenses.Update;

public class UpdateExpenseUseCase : IUpdateExpenseUseCase
{
    private readonly IExpensesUpdateOnlyRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILoggedUser _loggedUser;
    public UpdateExpenseUseCase(IExpensesUpdateOnlyRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILoggedUser loggedUser)
    {
        _repository = repository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _loggedUser = loggedUser;
    }

    public async Task Execute(long id, RequestExpenseJson request)
    {
        Validate(request);

        var loggedUser = await _loggedUser.Get();

        var expense = await _repository.GetById(loggedUser, id);

        if (expense is null) throw new NotFoundException(ResourceErrorMessages.EXPENSE_NOT_FOUND);

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Deleto todas as tags antigas para depois adicionar as novas
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        expense.Tags.Clear();

        _mapper.Map(request, expense); // O primeiro parâmetro é a fonte dos dados
                                       // O segundo é o destino desses dados, pra onde ele vai mapear

        _repository.Update(expense);

        await _unitOfWork.Commit();
    }

    private void Validate(RequestExpenseJson request)
    {
        var validator = new ExpenseValidator();
        var result = validator.Validate(request);

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
