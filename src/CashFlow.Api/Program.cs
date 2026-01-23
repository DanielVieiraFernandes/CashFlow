using CashFlow.Api.Filters;
using CashFlow.Api.Middleware;
using CashFlow.Api.Token;
using CashFlow.Application;
using CashFlow.Domain.Security.Tokens;
using CashFlow.Infraestructure;
using CashFlow.Infraestructure.DataAccess;
using CashFlow.Infraestructure.Extensions;
using CashFlow.Infraestructure.Migrations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(config =>
{
    // Aqui configuramos como essa opção irá aparecer noo swagger
    // e como ela vai enviar o token
    config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = @"JWT Authorization header using the Bearer scheme.
                        Enter 'Bearer' [space] and then your token in the text input below
                        Example: 'Bearer 1234abcdef'",
        In = ParameterLocation.Header,
        Scheme = "Bearer",
        Type = SecuritySchemeType.ApiKey,
    });

    config.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    }
    );
});
builder.Services.AddMvc(op => op.Filters.Add(typeof(ExceptionFilter)));
builder.Services.AddRouting(op =>
{
    op.LowercaseUrls = true;
    op.LowercaseQueryStrings = true;
});

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

// Registramos o HttpContextTokenValue como implementação da interface ITokenProvider
builder.Services.AddScoped<ITokenProvider, HttpContextTokenValue>();
//-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
// Necessário para o HttpContextTokenValue funcionar
// liberando a interface IHttpContextAccessor para injeção de dependência
//-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
builder.Services.AddHttpContextAccessor();

var signingKey = builder.Configuration.GetValue<string>("Settings:Jwt:SigningKey");

// A Api deve estar preparada para utilizar autenticação com essas configurações

// Define que a api deve utilizar uma autenticação no padrão JWT
builder.Services.AddAuthentication(config =>
{
    // Define os schemas padrões como JwtBearer
    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(config =>
{
    // Através desses parâmetros, será validado o token

    config.TokenValidationParameters = new TokenValidationParameters
    {
        // *-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
        // Desativamos essas opções pois não queremos validar
        // quem emitiu o token
        // ou para quem o token é destinado
        // *-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*

        ValidateIssuer = false,
        ValidateAudience = false,

        // Serve para evitar erros na comparação de data hora ao verificar se um token está expirado
        ClockSkew = new TimeSpan(0),

        // É necessário para validar a assinatura do token
        // Assim nossa aplicação verifica através dessa chave se o token válido
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey!))
    };

});

//**************************************************
// Adiciona os health checks na aplicação
// e verifica a conexão com o banco de dados
//**************************************************
builder.Services.AddHealthChecks().AddDbContextCheck<CashFlowDbContext>();

var app = builder.Build();

//-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
// Com isso verificamos se nossa API está executando e se 
// comunicando com o banco de dados corretamente
//-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*

// Vem do pacote Microsoft.AspNetCore.Diagnostics.HealthChecks
app.MapHealthChecks("/health", new HealthCheckOptions
{
    // Não queremos que faça o cache dessas respostas
    AllowCachingResponses = false,

    // Definimos como será o retorno do status code baseado no resultado
    ResultStatusCodes = {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status503ServiceUnavailable,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    },

});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<CultureMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

//******************************************************
// Somente se não estivermos em um ambiente de testes
// executamos as migrations do banco de dados
//******************************************************
if (!builder.Configuration.IsTestEnvironment())
    await MigrateDatabase();

app.Run();

async Task MigrateDatabase()
{
    // ================================================================================================
    // Criamos um escopo para simular uma chamada na API, assim, conseguimos
    // chamar os serviços de injeção de dependência.
    // ================================================================================================
    await using var scope = app.Services.CreateAsyncScope();

    await DatabaseMigration.MigrateDatabase(scope.ServiceProvider);
}

//************************************************************
// Classe parcial necessária para os testes de integração.
// Serve para expor o ponto de entrada da aplicação
//************************************************************
public partial class Program { }