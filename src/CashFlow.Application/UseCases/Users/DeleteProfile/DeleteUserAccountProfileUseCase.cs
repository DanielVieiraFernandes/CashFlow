
using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.User;
using CashFlow.Domain.Services.LoggedUser;

namespace CashFlow.Application.UseCases.Users.DeleteProfile;

public class DeleteUserAccountProfileUseCase : IDeleteUserAccountProfileUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IUserWriteOnlyRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserAccountProfileUseCase(
        ILoggedUser loggedUser,
        IUserWriteOnlyRepository repository,
        IUnitOfWork unitOfWork)
    {
        _loggedUser = loggedUser;
        _repository = repository;
        _unitOfWork = unitOfWork;
    }
    public async Task Execute()
    {
        var user = await _loggedUser.Get();

        await _repository.Delete(user);

        await _unitOfWork.Commit();
    }
}
