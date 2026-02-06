using Microsoft.EntityFrameworkCore;
using Notifications.API.Data;
using Notifications.API.Models;
using Notifications.API.DTOs;
using System.Text.Json;

namespace Notifications.API.Services;

/// <summary>
/// Servicio de background para generar notificaciones de recordatorio para tareas recurrentes
/// Verifica periódicamente las tareas recurrentes y crea notificaciones para las próximas ocurrencias
/// </summary>
public class RecurringNotificationService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RecurringNotificationService> _logger;
    private readonly HttpClient _httpClient;

    public RecurringNotificationService(
        IServiceProvider serviceProvider,
        ILogger<RecurringNotificationService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _httpClient = new HttpClient();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await GenerateRecurringNotifications(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en RecurringNotificationService");
            }

            // Ejecutar cada hora para generar notificaciones para las próximas ocurrencias
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

    private async Task GenerateRecurringNotifications(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var notificationsContext = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();

        try
        {
            // Obtener todas las tareas desde Tasks API
            // Usar el nombre del servicio de Docker Compose (tasks-api)
            var tasksUrl = Environment.GetEnvironmentVariable("TASKS_API_URL") ?? "http://tasks-api:8080";
            _logger.LogInformation("Obteniendo tareas desde: {Url}", tasksUrl);
            var tasksResponse = await _httpClient.GetAsync($"{tasksUrl}/api/tasks", cancellationToken);
            if (!tasksResponse.IsSuccessStatusCode)
            {
                _logger.LogWarning("No se pudo obtener tareas desde Tasks API. Status: {Status}", tasksResponse.StatusCode);
                return;
            }

            var tasksJson = await tasksResponse.Content.ReadAsStringAsync(cancellationToken);
            var tasks = JsonSerializer.Deserialize<List<TaskDto>>(tasksJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<TaskDto>();
            
            _logger.LogInformation("Obtenidas {Count} tareas para procesar notificaciones recurrentes", tasks.Count);

            var now = DateTime.UtcNow;
            var oneHourFromNow = now.AddHours(1);
            var oneYearFromNow = now.AddYears(1);

            foreach (var task in tasks)
            {
                foreach (var taskItem in task.Tasks)
                {
                    // Solo procesar tareas recurrentes (no "once")
                    var frequency = taskItem.Frequency?.ToLower() ?? "once";
                    if (frequency == "once")
                    {
                        continue; // Las tareas "once" ya tienen su notificación creada
                    }

                    if (string.IsNullOrEmpty(taskItem.Date) || string.IsNullOrEmpty(taskItem.Time))
                    {
                        continue; // Sin fecha/hora, no se puede generar
                    }

                    // Generar ocurrencias futuras
                    var occurrences = RecurringTaskHelper.GenerateOccurrences(taskItem, 365);
                    
                    foreach (var occurrence in occurrences)
                    {
                        // Solo crear notificaciones para ocurrencias en el futuro
                        if (occurrence.TaskDateTime <= now)
                        {
                            continue;
                        }

                        // Calcular ShowAt (1 hora antes de la ocurrencia)
                        var showAt = occurrence.TaskDateTime.AddHours(-1);

                        // Solo crear notificación si:
                        // 1. El showAt está en el futuro (al menos 1 hora desde ahora)
                        // 2. No existe ya una notificación para esta ocurrencia
                        if (showAt > now && showAt <= oneYearFromNow)
                        {
                            // Verificar si ya existe una notificación para esta ocurrencia
                            var existingNotification = await notificationsContext.Notifications
                                .FirstOrDefaultAsync(n => 
                                    n.UserId == task.UserId &&
                                    n.ReferenceId == task.Id &&
                                    n.TaskDateTime.HasValue &&
                                    Math.Abs((n.TaskDateTime.Value - occurrence.TaskDateTime).TotalMinutes) < 1 &&
                                    n.Status == "pending",
                                    cancellationToken);

                            if (existingNotification == null)
                            {
                                // Crear nueva notificación de recordatorio
                                var notification = new Notification
                                {
                                    Id = Guid.NewGuid(),
                                    UserId = task.UserId,
                                    Type = "task",
                                    ReferenceId = task.Id,
                                    Message = $"Recordatorio: {taskItem.Title} - Hora: {occurrence.Time} - Fecha: {occurrence.Date}",
                                    Status = "pending",
                                    TaskDateTime = occurrence.TaskDateTime,
                                    ShowAt = showAt,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow
                                };

                                notificationsContext.Notifications.Add(notification);
                                _logger.LogInformation(
                                    "Notificación de recordatorio creada para tarea recurrente: {TaskId}, Ocurrencia: {OccurrenceDate}, ShowAt: {ShowAt}",
                                    task.Id, occurrence.TaskDateTime, showAt);
                            }
                        }
                    }
                }
            }

            await notificationsContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Proceso de generación de notificaciones recurrentes completado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar notificaciones recurrentes");
        }
    }

    public override void Dispose()
    {
        _httpClient?.Dispose();
        base.Dispose();
    }
}
