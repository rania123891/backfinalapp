var builder = WebApplication.CreateBuilder(args);

// Ajouter Swagger
builder.Services.AddEndpointsApiExplorer(); // Important pour Swagger
builder.Services.AddSwaggerGen(); // Ajouter Swagger

// Ajouter YARP
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// Activer Swagger seulement en mode développement
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Activer le proxy
app.MapReverseProxy();

app.Run();
