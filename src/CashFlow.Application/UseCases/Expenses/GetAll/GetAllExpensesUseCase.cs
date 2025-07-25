using AutoMapper;
using CashFlow.Communication.Responses;
using CashFlow.Domain.Repositories.Expenses;

namespace CashFlow.Application.UseCases.Expenses.GetAll;
public class GetAllExpensesUseCase : IGetAllExpensesUseCase
{
    private readonly IExpensesReadonlyRepository _repository;
    private readonly IMapper _mapper;
    public GetAllExpensesUseCase(IExpensesReadonlyRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    public async Task<ResponseExpensesJson> Execute()
    {
        var result = await _repository.GetAll();

        return new ResponseExpensesJson
        {
            Expenses = _mapper.Map<List<ResponseShortExpenseJson>>(result)
        };
    }
}
