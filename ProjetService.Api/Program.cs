using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProjetService.Data.Context;
using ProjetService.Data.Repositories;
using ProjetService.Domain.Interfaces;
using System.Text;
using ProjetService.Domain.Interface;
using ProjetService.Infra.Services;
using System.Text.Json.Serialization;
using ProjetService.Infrastructure.Services;
using System.Security.Claims;
using ProjetService.Domain.Commands;

var builder = WebApplication.CreateBuilder(args);

// ? V�rifier si la cl� JWT est bien d�finie
var jwtSecret = builder.Configuration["Jwt:Secret"];
if (string.IsNullOrWhiteSpace(jwtSecret))
{
    throw new ArgumentNullException("Jwt:Secret", "? La cl� JWT est introuvable dans appsettings.json !");
}

// ? V�rifier la connexion � la base de donn�es
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("? ERREUR: La connexion � la base de donn�es est introuvable !");
}
Console.WriteLine("? Connexion � la base de donn�es bien configur�e.");

// ? Enregistrer le DbContext
builder.Services.AddDbContext<ProjetDbContext>(options =>
    options.UseSqlServer(connectionString));

// ? Enregistrer les d�pendances (Repositories, Services, etc.)
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<ICommandProcessor, CommandProcessor>();
builder.Services.AddScoped<ISaisieTempsRepository, SaisieTempsRepository>();


// ? Configurer MediatR
//builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<Program>();
    cfg.RegisterServicesFromAssemblyContaining<CreateProjetCommand>();
});

// ? Configurer l'authentification JWT
var key = Encoding.ASCII.GetBytes("VotreCleSecreteTresLongueEtComplexe");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        NameClaimType = ClaimTypes.NameIdentifier,
        RoleClaimType = ClaimTypes.Role
    };
});

// ? Ajouter CORS (si besoin d'acc�s cross-origin)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// ? Ajouter l'autorisation
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

// ? Ajouter Swagger pour l'API avec prise en charge de l'authentification JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ProjetService API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",  // en minuscules, important
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Entrer 'Bearer <token>' dans le champ"
    });


    c.AddSecurityRequirement(new OpenApiSecurityRequirement
{
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        new string[] {}
    }
});

});

// ? Ajouter les contr�leurs
// Dans Program.cs, apr�s builder.Services.AddControllers()
// Ajouter cette ligne dans votre Program.cs
builder.Services.AddScoped<IETAPredictionService, ETAPredictionService>();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // G�rer les cycles de r�f�rence
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

        // Optionnel : am�liorer la lisibilit�
        options.JsonSerializerOptions.WriteIndented = true;

        // Ignorer les propri�t�s null
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });
// Ajouter dans votre Program.cs existant :
builder.Services.AddScoped<IPlanificationRepository, PlanificationRepository>();

var app = builder.Build();

// ? Configurer le pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Debugging JWT - � SUPPRIMER APR�S
// Debugging JWT - � SUPPRIMER APR�S
app.Use(async (context, next) =>
{
    Console.WriteLine($"DEBUG - Requ�te re�ue: {context.Request.Method} {context.Request.Path}");
    Console.WriteLine($"DEBUG - Authorization header: {context.Request.Headers["Authorization"].FirstOrDefault()}");

    await next();

    Console.WriteLine($"DEBUG - R�ponse: {context.Response.StatusCode}");
});
app.UseRouting();
app.UseCors("AllowAllOrigins");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();