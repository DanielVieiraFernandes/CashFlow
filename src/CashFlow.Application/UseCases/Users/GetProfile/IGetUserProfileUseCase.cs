using CashFlow.Communication.Responses;

namespace CashFlow.Application.UseCases.Users.GetProfile;

public interface IGetUserProfileUseCase
{
    Task<ResponseUserProfileJson> Execute();
}
