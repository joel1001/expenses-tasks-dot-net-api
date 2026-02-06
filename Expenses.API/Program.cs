using Microsoft.EntityFrameworkCore;
using Expenses.API.Data;

// Configurar Npgsql para manejar timestamps correctamente
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Railway: escuchar en PORT
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Configurar JSON para usar camelCase (compatibilidad con frontend)
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// Add DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Host=localhost;Database=expenses_dev;Username=postgres;Password=postgres;Port=5434";

// Log connection string for debugging (hide password)
var safeConn = connectionString != null ? System.Text.RegularExpressions.Regex.Replace(connectionString, @"Password=[^;]*", "Password=***") : "null";
Console.WriteLine($"🔌 Connecting to PostgreSQL: {safeConn}");

builder.Services.AddDbContext<ExpensesDbContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions =>
        npgsqlOptions.EnableRetryOnFailure()));

var app = builder.Build();

// Configure the HTTP request pipeline.
// IMPORTANTE: El orden del middleware es crítico

// CORS debe ir primero
app.UseCors();

// Permitir HTTP en desarrollo (evitar problemas de certificados)
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Swagger siempre habilitado
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Expenses API V1");
    c.RoutePrefix = "swagger"; // Acceder en /swagger
});

app.UseAuthorization();
app.MapControllers();

// Crear schema/tablas si no existen (Neon o Postgres local)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ExpensesDbContext>();
    if (db.Database.CanConnect())
        db.Database.EnsureCreated();
}

app.Run();
