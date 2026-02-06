using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Notifications.API.Data;
using Notifications.API.Models;

namespace Notifications.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly NotificationsDbContext _context;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(NotificationsDbContext context, ILogger<NotificationsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Notification>>> GetNotifications()
    {
        return await _context.Notifications.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Notification>> GetNotification(Guid id)
    {
        var notification = await _context.Notifications.FindAsync(id);

        if (notification == null)
        {
            return NotFound();
        }

        return notification;
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<Notification>>> GetNotificationsByUser(Guid userId)
    {
        // Retornar solo notificaciones que ya se han activado (sent) o que ya fueron leídas (read)
        // No mostrar notificaciones "pending" que aún no se han activado
        return await _context.Notifications
            .Where(n => n.UserId == userId && (n.Status == "sent" || n.Status == "read"))
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    [HttpGet("user/{userId}/unread")]
    public async Task<ActionResult<IEnumerable<Notification>>> GetUnreadNotificationsByUser(Guid userId)
    {
        // Solo contar notificaciones con status "sent" (ya activadas) que no han sido leídas
        // Las notificaciones "pending" aún no se han mostrado, por lo que no se cuentan
        return await _context.Notifications
            .Where(n => n.UserId == userId && n.Status == "sent")
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    [HttpGet("user/{userId}/unread/count")]
    public async Task<ActionResult<int>> GetUnreadNotificationsCount(Guid userId)
    {
        // Solo contar notificaciones con status "sent" (ya activadas) que no han sido leídas
        // Las notificaciones "pending" aún no se han mostrado, por lo que no se cuentan como no leídas
        var count = await _context.Notifications
            .CountAsync(n => n.UserId == userId && n.Status == "sent");
        return count;
    }

    [HttpPost]
    public async Task<ActionResult<Notification>> CreateNotification(Notification notification)
    {
        notification.CreatedAt = DateTime.UtcNow;
        notification.UpdatedAt = DateTime.UtcNow;
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetNotification), new { id = notification.Id }, notification);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateNotification(Guid id, Notification notification)
    {
        if (id != notification.Id)
        {
            return BadRequest();
        }

        notification.UpdatedAt = DateTime.UtcNow;
        _context.Entry(notification).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!NotificationExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpPatch("{id}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var notification = await _context.Notifications.FindAsync(id);
        if (notification == null)
        {
            return NotFound();
        }

        notification.Status = "read";
        notification.ReadAt = DateTime.UtcNow;
        notification.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNotification(Guid id)
    {
        var notification = await _context.Notifications.FindAsync(id);
        if (notification == null)
        {
            return NotFound();
        }

        _context.Notifications.Remove(notification);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool NotificationExists(Guid id)
    {
        return _context.Notifications.Any(e => e.Id == id);
    }
}
