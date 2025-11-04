using CashFlow.Application.UseCases.Login.DoLogin;
using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;
using Microsoft.AspNetCore.Mvc;

namespace CashFlow.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class LoginController : ControllerBase
{

    [HttpPost]
    [ProducesResponseType<ResponseRegisteredUserJson>(StatusCodes.Status200OK)]
    [ProducesResponseType<ResponseErrorJson>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromServices] IDoLoginUseCase useCase, [FromBody] RequestLoginJson request)
    {
        var result = await useCase.Execute(request);

        return Ok(result);
    }
}
