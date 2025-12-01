using CashFlow.Domain.Entities;
using CashFlow.Domain.Security.Tokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CashFlow.Infraestructure.Security;
public class JwtTokenGenerator : IAccessTokenGenerator
{
    private readonly uint _expirationTimeMinutes;
    private readonly string _signingKey;

    public JwtTokenGenerator(uint expirationTimeMinutes, string signingKey)
    {
        _expirationTimeMinutes = expirationTimeMinutes;
        _signingKey = signingKey;
    }
    public string Generate(User user)
    {
        //+++++++++++++++++++++++++++++++++++++++++++++++
        // Descreve como o token JWT deve ser gerado:
        //+++++++++++++++++++++++++++++++++++++++++++++++

        // Aqui guardamos as informações que serão armazenadas no token

        /*
         NÃO É OBRIGATÓRIO UTILIZAR O ClaimTypes, É APENAS UMA STRING,
         PODEMOS DIGITAR UMA STRING LIVREMENTE -> ex:
         
            new Claim("nome", user.Name)

         */
        var claims = new List<Claim>()
        {
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Sid, user.UserIdentifier.ToString()),
        };

        var tokenDescritptor = new SecurityTokenDescriptor
        {
            // Configura quando o token deve expirar
            Expires = DateTime.UtcNow.AddMinutes(_expirationTimeMinutes),

            // Configura o algoritmo usado para assinar o token
            SigningCredentials = new SigningCredentials(SecurityKey(), SecurityAlgorithms.HmacSha256Signature),

            // É aqui que colocamos as propriedades desejadas no token
            Subject = new ClaimsIdentity(claims)
        };

        //+++++++++++++++++++++++++++++++++++++++++++++++
        // Cria o token JWT
        //+++++++++++++++++++++++++++++++++++++++++++++++

        var tokenHandler = new JwtSecurityTokenHandler();

        var securityToken = tokenHandler.CreateToken(tokenDescritptor);

        // Devolve o token como string
        return tokenHandler.WriteToken(securityToken);
    }

    private SymmetricSecurityKey SecurityKey()
    {
        // Recupera os bytes da chave
        var key = Encoding.UTF8.GetBytes(_signingKey);

        // Retorna uma instância da classe necessária para passar a chave nas credenciais do token
        return new SymmetricSecurityKey(key);
    }
}
