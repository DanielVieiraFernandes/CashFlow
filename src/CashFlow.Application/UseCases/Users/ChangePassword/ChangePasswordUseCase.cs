using CashFlow.Communication.Requests;
using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.User;
using CashFlow.Domain.Security.Cryptography;
using CashFlow.Domain.Services.LoggedUser;
using CashFlow.Exception;
using CashFlow.Exception.ExceptionsBase;
using FluentValidation.Results;

namespace CashFlow.Application.UseCases.Users.ChangePassword;

public class ChangePasswordUseCase : IChangePasswordUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IUserUpdateOnlyRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordEncrypter _passwordEncrypter;
    public ChangePasswordUseCase(
        IUnitOfWork unitOfWork,
        IPasswordEncrypter passwordEncrypter,
        IUserUpdateOnlyRepository repository,
        ILoggedUser loggedUser
        )
    {
        _loggedUser = loggedUser;
        _unitOfWork = unitOfWork;
        _passwordEncrypter = passwordEncrypter;
        _repository = repository;
    }
    public async Task Execute(RequestChangePasswordJson request)
    {
        var loggedUser = await _loggedUser.Get();

        Validate(request, loggedUser);

        var user = await _repository.GetById(loggedUser.Id);
        user.Password = _passwordEncrypter.Encrypt(request.NewPassword);

        _repository.Update(user);

        await _unitOfWork.Commit();
    }

    private void Validate(RequestChangePasswordJson request, Domain.Entities.User loggedUser)
    {
        var validator = new ChangePasswordValidator();

        var result = validator.Validate(request);

        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
        // Verifico se a senha enviada é igual a senha atual do usuário
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
        var passwordMatch = _passwordEncrypter.Verify(request.Password, loggedUser.Password);

        if (!passwordMatch)
        {
            result.Errors.Add(new ValidationFailure(string.Empty,
                ResourceErrorMessages.PASSWORD_DIFFERENT_CURRENT_PASSWORD));
        }

        if (!result.IsValid)
        {
            var errors = result.Errors.Select(e => e.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errors);
        }
    }
}
