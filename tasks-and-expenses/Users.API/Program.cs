using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Users.API.Application.Behaviors;
using Users.API.Application.Interfaces;
using Users.API.Data;
using Users.API.Infrastructure.Repositories;

// Configurar Npgsql para manejar timestamps correctamente
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Host=localhost;Database=users_dev;Username=postgres;Password=postgres;Port=5432";

// Log connection string for debugging (without password)
Console.WriteLine($"🔌 Connecting to PostgreSQL at: {connectionString.Replace("Password=postgres", "Password=***")}");

// Log connection string for debugging (remove in production)
Console.WriteLine($"🔌 Connection String: {connectionString?.Replace("Password=postgres", "Password=***")}");

builder.Services.AddDbContext<UsersDbContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions =>
        npgsqlOptions.EnableRetryOnFailure()));

// Register interfaces and implementations (SOLID - Dependency Inversion)
builder.Services.AddScoped<IUsersDbContext>(provider => provider.GetService<UsersDbContext>()!);
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Add MediatR for CQRS
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    // Add validation behavior pipeline
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
});

// Add FluentValidation
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

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
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Users API V1");
    c.RoutePrefix = "swagger"; // Acceder en /swagger
});

// MapControllers - debe estar al final
app.MapControllers();

app.Run();

// Make Program accessible for integration tests
public partial class Program { }
