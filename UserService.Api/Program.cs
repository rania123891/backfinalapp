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

// Middleware de logging personnalisé
builder.Logging.AddConsole();
var logger = LoggerFactory.Create(config =>
{
    config.AddConsole();
}).CreateLogger("Program");

// Vérifier si la clé JWT est bien définie
var jwtSecret = builder.Configuration["Jwt:Secret"]
    ?? throw new ArgumentNullException("Jwt:Secret", "⚠️ La clé JWT est introuvable dans appsettings.json !");

logger.LogInformation("🔐 Configuration JWT initialisée");

// Enregistrer le DbContext
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("UserDb")));

// Enregistrer les dépendances
builder.Services.AddScoped<IUtilisateurRepository, UtilisateurRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Enregistrer MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining<UserService.Domain.Commands.CreateUtilisateurCommand>());

// Configurer JWT avec logging détaillé
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
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        NameClaimType = "nameid"
    };

    // Événements de debugging JWT
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            logger.LogError($"⚠️ Échec d'authentification: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            logger.LogInformation($"✅ Token validé pour l'utilisateur: {context.Principal?.Identity?.Name}");
            return Task.CompletedTask;
        },
        OnMessageReceived = context =>
        {
            logger.LogInformation($"🔍 Token reçu dans la requête");
            return Task.CompletedTask;
        }
    };
});

// Configuration CORS améliorée
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy => policy
            .WithOrigins(
                "http://localhost:4200", // Angular dev server
                "http://localhost:4000"  // Production si nécessaire
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .WithExposedHeaders("Authorization")
    );
});

// Autorisation
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin", "Administrateur"));
});

// Services Web
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();

// Configuration Swagger
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
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
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
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Middleware de logging des requêtes
app.Use(async (context, next) =>
{
    logger.LogInformation($"📝 Requête entrante: {context.Request.Method} {context.Request.Path}");
    var authHeader = context.Request.Headers["Authorization"].ToString();
    if (!string.IsNullOrEmpty(authHeader))
    {
        logger.LogInformation("🔑 Token présent dans la requête");
    }

    await next();

    logger.LogInformation($"📤 Réponse: {context.Response.StatusCode}");
});

// Configuration de l'environnement
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    logger.LogInformation("🔧 Mode développement activé");
}

// Configuration du pipeline HTTP
app.UseRouting();

// CORS doit être entre UseRouting et UseAuthentication
app.UseCors("AllowAngularApp");
logger.LogInformation("🌐 CORS configuré");

// Fichiers statiques pour les images
app.UseStaticFiles();
logger.LogInformation("📁 Fichiers statiques configurés");

// Sécurité
app.UseAuthentication();
app.UseAuthorization();
logger.LogInformation("🔒 Authentification et autorisation configurées");

// Routes
app.MapControllers();

logger.LogInformation("🚀 Application démarrée");
app.Run();