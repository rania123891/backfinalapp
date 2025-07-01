using MessageService.Data;
using MessageService.Domain.Interfaces;
using MessageService.Infra.Services;
using MessageService.Data.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuration Entity Framework
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MessageDb")));

// Services Domain et Data
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();

// MediatR pour CQRS
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

// HttpClient pour les appels vers UserService
builder.Services.AddHttpClient();

// Controllers
builder.Services.AddControllers();

// Configuration upload de fichiers
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 10 * 1024 * 1024; // 10MB
});

builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10MB
    options.ValueLengthLimit = int.MaxValue;
    options.ValueCountLimit = int.MaxValue;
    options.KeyLengthLimit = int.MaxValue;
});

// CORS pour Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowAngularApp");
app.MapControllers();

// Messages de démarrage
Console.WriteLine("🚀 MessageService.Api démarré");
Console.WriteLine("📁 Service de fichiers activé");
Console.WriteLine($"📍 Test API : https://localhost:{(app.Environment.IsDevelopment() ? "7269" : "5140")}/api/Messages");
Console.WriteLine($"📊 Swagger : https://localhost:{(app.Environment.IsDevelopment() ? "7269" : "5140")}/swagger");

app.Run();