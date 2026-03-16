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

// Railway: escuchar en PORT
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

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
// Timeout de conexión: 30s para dar tiempo a cold start de Cloud SQL
if (!connectionString.Contains("Timeout=", StringComparison.OrdinalIgnoreCase))
    connectionString = connectionString.TrimEnd(';') + ";Timeout=30";

// Log connection string for debugging (hide password)
var safeConn = connectionString != null ? System.Text.RegularExpressions.Regex.Replace(connectionString, @"Password=[^;]*", "Password=***") : "null";
Console.WriteLine($"🔌 Connecting to PostgreSQL: {safeConn}");

builder.Services.AddDbContext<UsersDbContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.CommandTimeout(30);
        // Reintentos para cold start de Cloud SQL y fallos transitorios
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorCodesToAdd: null);
    }));

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

// Capturar cualquier excepción y devolver JSON con mensaje (evitar 500 sin cuerpo que el FE no puede mostrar)
app.Use(async (ctx, next) =>
{
    try
    {
        await next(ctx);
    }
    catch (Exception ex)
    {
        ctx.Response.StatusCode = 500;
        ctx.Response.ContentType = "application/json";
        var inner = ex.InnerException ?? ex;
        var msg = ex.Message + " " + inner.Message;
        var isDb = ex.Source?.Contains("Npgsql") == true || inner.Source?.Contains("Npgsql") == true
            || msg.Contains("connection", StringComparison.OrdinalIgnoreCase)
            || msg.Contains("timeout", StringComparison.OrdinalIgnoreCase)
            || msg.Contains("retries", StringComparison.OrdinalIgnoreCase);
        var message = isDb
            ? "No se pudo conectar a la base de datos. Revisa Cloud SQL (red autorizada 0.0.0.0/0) y la contraseña en Cloud Run."
            : (app.Environment.IsDevelopment() ? ex.Message : "Error interno. Revisa los logs del servicio.");
        await ctx.Response.WriteAsJsonAsync(new { error = message });
    }
});

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

// Health check: 200 en cuanto el proceso escucha (Cloud Run no mata la instancia)
app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "users-api" }));

// Sin 503: todas las peticiones /api pasan al controller. Si la BD no está lista, el controller devuelve 500 con mensaje claro.
// MapControllers
app.MapControllers();

// Inicializar BD en segundo plano: la app ya escucha, no bloquea arranque
_ = Task.Run(async () =>
{
    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(4));
    try
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
        if (await db.Database.CanConnectAsync(cts.Token))
            await db.Database.EnsureCreatedAsync(cts.Token);
        Users.API.Startup.DbReady = true;
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("[Users.API] DB init timeout (4s), accepting requests anyway.");
        Users.API.Startup.DbReady = true;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[Users.API] DB init failed: {ex.Message}");
        Users.API.Startup.DbReady = true;
    }
});

app.Run();

// Make Program accessible for integration tests
public partial class Program { }
