using Microsoft.EntityFrameworkCore;
using Notifications.API.Data;
using Notifications.API.Models;

namespace Notifications.API.Services;

public class TaskNotificationService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TaskNotificationService> _logger;
    private readonly HttpClient _httpClient;

    public TaskNotificationService(
        IServiceProvider serviceProvider,
        ILogger<TaskNotificationService> logger)
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
                await CheckAndActivateNotifications(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en TaskNotificationService");
            }

            // Esperar 1 minuto antes de la siguiente verificación
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private async Task CheckAndActivateNotifications(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();

        var now = DateTime.UtcNow;
        var fifteenMinutesFromNow = now.AddMinutes(15);

        // Buscar notificaciones que deben mostrarse ahora (ShowAt <= now y Status = "pending")
        var notificationsToActivate = await context.Notifications
            .Where(n => n.ShowAt != null 
                     && n.ShowAt <= now 
                     && n.Status == "pending"
                     && n.Type == "task")
            .ToListAsync(cancellationToken);

        foreach (var notification in notificationsToActivate)
        {
            // Cambiar status a "sent" para que aparezca como no leída
            notification.Status = "sent";
            notification.UpdatedAt = DateTime.UtcNow;
            _logger.LogInformation("Notificación activada: {NotificationId} para usuario {UserId}", 
                notification.Id, notification.UserId);
        }

        if (notificationsToActivate.Any())
        {
            await context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Se activaron {Count} notificaciones", notificationsToActivate.Count);
        }
    }

    public override void Dispose()
    {
        _httpClient?.Dispose();
        base.Dispose();
    }
}
