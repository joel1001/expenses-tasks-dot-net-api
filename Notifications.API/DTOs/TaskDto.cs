using System.Text.Json.Serialization;

namespace Notifications.API.DTOs;

public class TaskDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    
    [JsonPropertyName("userId")]
    public Guid UserId { get; set; }
    
    [JsonPropertyName("tasks")]
    public List<TaskItemDto> Tasks { get; set; } = new();
    
    [JsonPropertyName("completedTasks")]
    public List<TaskItemDto> CompletedTasks { get; set; } = new();
    
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
    
    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}

public class TaskItemDto
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    
    [JsonPropertyName("title")]
    public string? Title { get; set; }
    
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    
    [JsonPropertyName("date")]
    public string? Date { get; set; }
    
    [JsonPropertyName("time")]
    public string? Time { get; set; }
    
    [JsonPropertyName("frequency")]
    public string? Frequency { get; set; } = "once";
    
    [JsonPropertyName("createdAt")]
    public string? CreatedAt { get; set; }
}
