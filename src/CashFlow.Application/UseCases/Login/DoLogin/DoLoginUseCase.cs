using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;
using CashFlow.Domain.Repositories.User;
using CashFlow.Domain.Security.Cryptography;
using CashFlow.Domain.Security.Tokens;
using CashFlow.Exception.ExceptionsBase;

namespace CashFlow.Application.UseCases.Login.DoLogin;
public class DoLoginUseCase : IDoLoginUseCase
{
    private readonly IPasswordEncrypter _passwordEncrypter;
    private readonly IAccessTokenGenerator _accessTokenGenerator;
    private readonly IUserReadOnlyRepository _repository;

    public DoLoginUseCase(
            IPasswordEncrypter passwordEncrypter,
            IAccessTokenGenerator accessTokenGenerator,
            IUserReadOnlyRepository repository)
    {
        _passwordEncrypter = passwordEncrypter;
        _accessTokenGenerator = accessTokenGenerator;
        _repository = repository;
    }
    public async Task<ResponseRegisteredUserJson> Execute(RequestLoginJson request)
    {
        Validate(request);

        var user = await _repository.GetUserByEmail(request.Email);

        if (user is null)
            throw new InvalidLoginException();

        var passwordMatch = _passwordEncrypter.Verify(request.Password, user.Password);

        if (!passwordMatch)
            throw new InvalidLoginException();

        return new ResponseRegisteredUserJson
        {
            Name = user.Name,
            Token = _accessTokenGenerator.Generate(user)
        };
    }

    public void Validate(RequestLoginJson request)
    {
        var result = new DoLoginValidator().Validate(request);

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
