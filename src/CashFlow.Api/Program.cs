using CashFlow.Api.Filters;
using CashFlow.Api.Middleware;
using CashFlow.Application;
using CashFlow.Infraestructure;
using CashFlow.Infraestructure.Migrations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<CultureMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseAuthentication();

app.MapControllers();

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