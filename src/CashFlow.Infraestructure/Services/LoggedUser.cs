using CashFlow.Domain.Entities;
using CashFlow.Domain.Security.Tokens;
using CashFlow.Domain.Services.LoggedUser;
using CashFlow.Infraestructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CashFlow.Infraestructure.Services;

public class LoggedUser : ILoggedUser
{
    private readonly CashFlowDbContext _dbContext;
    private readonly ITokenProvider _tokenProvider;
    public LoggedUser(CashFlowDbContext dbContext, ITokenProvider tokenProvider)
    {
        _dbContext = dbContext;
        _tokenProvider = tokenProvider;
    }

    public Task<User> Get()
    {
        //**************************************************************
        // Recupero o token JWT a partir do provedor de tokens
        //**************************************************************
        string token = _tokenProvider.TokenOnRequest();

        //**************************************************************
        // Instancio a classe JwtSecurityTokenHandler
        //**************************************************************
        var tokenHandler = new JwtSecurityTokenHandler();

        //**************************************************************
        // Leio o token JWT passado como parâmetro
        //**************************************************************
        var jwtSecurityToken = tokenHandler.ReadJwtToken(token);

        //****************************************************************************
        // Recupero o identificador do usuário a partir das claim Sid no token JWT
        //****************************************************************************
        var identifier = jwtSecurityToken.Claims.First(claim => claim.Type == ClaimTypes.Sid).Value;

        //*************************************************
        // Recupero o usuário no banco de dados cujo o
        // identificador é igual ao recuperado no token
        //*************************************************
        return _dbContext.Users.AsNoTracking().FirstAsync(user => user.UserIdentifier == Guid.Parse(identifier));
    }
}
