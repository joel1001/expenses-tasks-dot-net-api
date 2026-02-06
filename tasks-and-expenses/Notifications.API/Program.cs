using Microsoft.EntityFrameworkCore;
using Notifications.API.Data;
using Notifications.API.Services;

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
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// Add DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Host=localhost;Database=notifications_dev;Username=postgres;Password=postgres;Port=5435";

builder.Services.AddDbContext<NotificationsDbContext>(options =>
    options.UseNpgsql(connectionString));

// Add Background Services
builder.Services.AddHostedService<TaskNotificationService>();
builder.Services.AddHostedService<RecurringNotificationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
// Manejar OPTIONS (preflight) ANTES de cualquier otro middleware
app.Use(async (context, next) =>
{
    if (context.Request.Method == "OPTIONS")
    {
        context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
        context.Response.Headers.Append("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, PATCH, OPTIONS");
        context.Response.Headers.Append("Access-Control-Allow-Headers", "Content-Type, Authorization");
        context.Response.StatusCode = 200;
        await context.Response.CompleteAsync();
        return;
    }
    await next();
});

app.UseCors();
app.UseRouting();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // No redirigir HTTPS en desarrollo para evitar problemas con CORS
}
else
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.MapControllers();

// Crear schema/tablas si no existen (Neon o Postgres local)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();
    if (db.Database.CanConnect())
        db.Database.EnsureCreated();
}

app.Run();
