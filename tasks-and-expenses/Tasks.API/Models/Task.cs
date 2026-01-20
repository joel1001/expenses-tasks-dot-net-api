using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tasks.API.Models;

public class Task
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid UserId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    [Column(TypeName = "jsonb")]
    public string CompletedTasks { get; set; } = "[]";
    
    [Column(TypeName = "jsonb")]
    public string Tasks { get; set; } = "[]";
}
