using MessageService.Data;
using MessageService.Data.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Ajout DbContext avec SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MessageDb")));

// Ajout du repository
builder.Services.AddScoped<IMessageRepository, MessageRepository>();

// Ajout de MediatR (pour les commandes/queries)
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(IMessageRepository).Assembly));

// Ajout Controllers et Swagger
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
