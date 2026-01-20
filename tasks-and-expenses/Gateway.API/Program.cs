using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Ocelot
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);

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

var app = builder.Build();

// Configure the HTTP request pipeline
// IMPORTANTE: El orden del middleware es crítico

// CORS debe ir primero
app.UseCors();

// Permitir HTTP en desarrollo
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Swagger siempre habilitado
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gateway API V1");
    c.RoutePrefix = "swagger"; // Acceder en /swagger
});

// MapControllers - antes de Ocelot
app.MapControllers();

// Add Ocelot middleware
await app.UseOcelot();

app.Run();
