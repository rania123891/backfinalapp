using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MediatR;
using Microsoft.OpenApi.Models;
using UserService.Data.Context;
using UserService.Data.Repositories;
using UserService.Domain.Interfaces;
using UserService.Infra.Services;
using UserService.Api.Filters;
using UserService.Api.Controllers;
using UserService.Domain.Interfaces.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Vérifier si la clé JWT est bien définie
var jwtSecret = builder.Configuration["Jwt:Secret"]
    ?? throw new ArgumentNullException("Jwt:Secret", "⚠️ La clé JWT est introuvable dans appsettings.json !");

// Enregistrer le DbContext
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("UserDb")));

// Enregistrer les dépendances
builder.Services.AddScoped<IUtilisateurRepository, UtilisateurRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Enregistrer MediatR en scannant l'assembly contenant vos commandes et handlers
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<UserService.Domain.Commands.CreateUtilisateurCommand>());

// Configurer JWT
var key = Encoding.ASCII.GetBytes(jwtSecret);
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
        NameClaimType = "nameid"
    };
});

// Ajouter CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// Ajouter l'autorisation avec la policy "AdminOnly"
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

// Ajouter les contrôleurs et configurer Swagger avec sécurité
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "UserService API",
        Version = "v1",
        Description = "API pour la gestion des utilisateurs"
    });

    c.SchemaFilter<FormFileSchemaFilter>();
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Entrez 'Bearer' [espace] et le token JWT.\r\nExemple: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

// Configurer le pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors("AllowAllOrigins");

// 🔧 AJOUT : Configurer les fichiers statiques pour servir les images uploadées
app.UseStaticFiles(); // Servira automatiquement les fichiers depuis le dossier wwwroot

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();