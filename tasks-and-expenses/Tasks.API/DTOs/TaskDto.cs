using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Tasks.API.DTOs;

public class TaskDto
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    
    public List<TaskItemDto> Tasks { get; set; } = new();
    
    public List<TaskItemDto> CompletedTasks { get; set; } = new();
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
}

public class TaskItemDto
{
    public string? Id { get; set; }
    
    public string? Title { get; set; }
    
    public string? Description { get; set; }
    
    // Campos de fecha y hora de la tarea
    [JsonPropertyName("date")]
    public string? Date { get; set; }
    
    [JsonPropertyName("time")]
    public string? Time { get; set; }
    
    [JsonPropertyName("createdAt")]
    public string? CreatedAt { get; set; }
    
    // Frecuencia de repetición de la tarea
    [JsonPropertyName("frequency")]
    public string? Frequency { get; set; } = "once"; // once, daily, weekly, biweekly, monthly
    
    // Campos de valores esperados y actuales en diferentes monedas
    // Acepta múltiples formatos: expectedUSD, Expected USD, etc.
    // NOTA: JsonExtensionData capturará campos con espacios y los preservará
    [JsonPropertyName("expectedUSD")]
    public decimal? ExpectedUSD { get; set; }
    
    [JsonPropertyName("actualUSD")]
    public decimal? ActualUSD { get; set; }
    
    [JsonPropertyName("expectedCRC")]
    public decimal? ExpectedCRC { get; set; }
    
    [JsonPropertyName("actualCRC")]
    public decimal? ActualCRC { get; set; }
    
    // Estos campos alternativos también se mapean desde AdditionalProperties si vienen con espacios
    // El JsonExtensionData capturará "Expected USD", "Actual USD", etc. cuando vengan del frontend
    
    // Otros campos que puedan venir del frontend se capturan aquí
    // Esto asegura que todos los campos editables, incluyendo "Expected USD", "Actual USD", etc., se preserven
    [System.Text.Json.Serialization.JsonExtensionData]
    public Dictionary<string, object>? AdditionalProperties { get; set; }
}

public class CreateTaskDto
{
    [Required]
    public Guid UserId { get; set; }
    
    public List<TaskItemDto> Tasks { get; set; } = new();
    
    public List<TaskItemDto> CompletedTasks { get; set; } = new();
}