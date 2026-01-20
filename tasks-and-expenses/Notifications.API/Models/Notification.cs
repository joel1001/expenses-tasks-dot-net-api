using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Notifications.API.Models;

public class Notification
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string Type { get; set; } = string.Empty; // "task" or "expense"
    
    [Required]
    public Guid ReferenceId { get; set; } // ID of the related task or expense
    
    [Required]
    public string Message { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string Status { get; set; } = "pending"; // "pending", "sent", "read"
    
    public DateTime? ReadAt { get; set; }
    
    // Campos para notificaciones de tareas programadas
    public DateTime? TaskDateTime { get; set; } // Fecha y hora de la tarea
    public DateTime? ShowAt { get; set; } // Cuándo mostrar la notificación (15 min antes de TaskDateTime)
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
