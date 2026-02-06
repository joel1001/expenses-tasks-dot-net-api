using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tasks.API.Data;
using Tasks.API.DTOs;
using TaskModel = Tasks.API.Models.Task;
using System.Text.Json;

namespace Tasks.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly TasksDbContext _context;
    private readonly ILogger<TasksController> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = false,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never,
        // Permitir comentarios y trailing commas
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };
    
    // Opciones para deserializar JSON sin modificar nombres (preservar espacios)
    private static readonly JsonSerializerOptions PreserveNamesOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = false,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    public TasksController(TasksDbContext context, ILogger<TasksController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpOptions]
    public IActionResult Options()
    {
        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasks()
    {
        var tasks = await _context.Tasks.ToListAsync();
        
        var dtos = tasks.Select(t => new TaskDto
        {
            Id = t.Id,
            UserId = t.UserId,
            Tasks = JsonSerializer.Deserialize<List<TaskItemDto>>(t.Tasks, JsonOptions) ?? new(),
            CompletedTasks = JsonSerializer.Deserialize<List<TaskItemDto>>(t.CompletedTasks, JsonOptions) ?? new(),
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt
        }).ToList();

        return dtos;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskDto>> GetTask(Guid id)
    {
        var task = await _context.Tasks.FindAsync(id);

        if (task == null)
        {
            return NotFound();
        }

        var dto = new TaskDto
        {
            Id = task.Id,
            UserId = task.UserId,
            Tasks = JsonSerializer.Deserialize<List<TaskItemDto>>(task.Tasks, JsonOptions) ?? new(),
            CompletedTasks = JsonSerializer.Deserialize<List<TaskItemDto>>(task.CompletedTasks, JsonOptions) ?? new(),
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };

        return dto;
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasksByUser(Guid userId)
    {
        var tasks = await _context.Tasks
            .Where(t => t.UserId == userId)
            .ToListAsync();
        
        var dtos = tasks.Select(t => new TaskDto
        {
            Id = t.Id,
            UserId = t.UserId,
            Tasks = JsonSerializer.Deserialize<List<TaskItemDto>>(t.Tasks, JsonOptions) ?? new(),
            CompletedTasks = JsonSerializer.Deserialize<List<TaskItemDto>>(t.CompletedTasks, JsonOptions) ?? new(),
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt
        }).ToList();

        return dtos;
    }

    [HttpPost]
    public async Task<ActionResult<TaskDto>> CreateTask([FromBody] CreateTaskDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var tasksJson = JsonSerializer.Serialize(dto.Tasks ?? new List<TaskItemDto>(), JsonOptions);
        var completedTasksJson = JsonSerializer.Serialize(dto.CompletedTasks ?? new List<TaskItemDto>(), JsonOptions);

        _logger.LogInformation("CreateTask - Tasks JSON: {TasksJson}", tasksJson);
        _logger.LogInformation("CreateTask - CompletedTasks JSON: {CompletedTasksJson}", completedTasksJson);

        var task = new TaskModel
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            Tasks = tasksJson,
            CompletedTasks = completedTasksJson,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        var response = new TaskDto
        {
            Id = task.Id,
            UserId = task.UserId,
            Tasks = dto.Tasks ?? new List<TaskItemDto>(),
            CompletedTasks = dto.CompletedTasks ?? new List<TaskItemDto>(),
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };

        return CreatedAtAction(nameof(GetTask), new { id = task.Id }, response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(Guid id, [FromBody] CreateTaskDto dto)
    {
        // Log del request completo para debugging
        _logger.LogInformation("UpdateTask - Recibido request para Task ID: {Id}", id);
        _logger.LogInformation("UpdateTask - Tasks count: {Count}, CompletedTasks count: {CompletedCount}", 
            dto.Tasks?.Count ?? 0, dto.CompletedTasks?.Count ?? 0);
        
        // Log de cada task item para ver qué campos vienen
        if (dto.Tasks != null)
        {
            for (int i = 0; i < dto.Tasks.Count; i++)
            {
                var taskItem = dto.Tasks[i];
                _logger.LogInformation("UpdateTask - Task[{Index}]: Id={Id}, Title={Title}, ExpectedUSD={ExpectedUSD}, ActualUSD={ActualUSD}, ExpectedCRC={ExpectedCRC}, ActualCRC={ActualCRC}, AdditionalPropsCount={AdditionalCount}",
                    i, taskItem.Id, taskItem.Title, taskItem.ExpectedUSD, taskItem.ActualUSD, 
                    taskItem.ExpectedCRC, taskItem.ActualCRC, taskItem.AdditionalProperties?.Count ?? 0);
                
                if (taskItem.AdditionalProperties != null && taskItem.AdditionalProperties.Count > 0)
                {
                    _logger.LogInformation("UpdateTask - Task[{Index}] AdditionalProperties: {Props}",
                        i, string.Join(", ", taskItem.AdditionalProperties.Select(kvp => $"{kvp.Key}={kvp.Value}")));
                }
            }
        }
        
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("UpdateTask - ModelState inválido: {Errors}", 
                string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
            return BadRequest(ModelState);
        }

        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
        {
            return NotFound();
        }

        // Procesar y preservar todos los campos, incluyendo los que están en AdditionalProperties
        // Esto asegura que campos como "Expected USD", "Actual USD", etc. se guarden correctamente
        var processedTasks = ProcessTaskItems(dto.Tasks ?? new List<TaskItemDto>());
        var processedCompletedTasks = ProcessTaskItems(dto.CompletedTasks ?? new List<TaskItemDto>());

        // Serializar preservando todos los campos, incluyendo AdditionalProperties
        // Usar opciones que preserven los nombres originales de los campos adicionales
        var tasksJson = JsonSerializer.Serialize(processedTasks, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            WriteIndented = false,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never,
            // Serializar AdditionalProperties en el JSON final
            Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
        });
        
        var completedTasksJson = JsonSerializer.Serialize(processedCompletedTasks, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            WriteIndented = false,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never,
            Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
        });

        _logger.LogInformation("UpdateTask - Task ID: {Id}, Tasks JSON: {TasksJson}", id, tasksJson);
        _logger.LogInformation("UpdateTask - CompletedTasks JSON: {CompletedTasksJson}", completedTasksJson);

        // Verificar que los campos Expected/Actual están en el JSON
        var jsonCombined = tasksJson + completedTasksJson;
        if (jsonCombined.Contains("expectedUSD", StringComparison.OrdinalIgnoreCase) || 
            jsonCombined.Contains("actualUSD", StringComparison.OrdinalIgnoreCase) ||
            jsonCombined.Contains("expectedCRC", StringComparison.OrdinalIgnoreCase) ||
            jsonCombined.Contains("actualCRC", StringComparison.OrdinalIgnoreCase) ||
            jsonCombined.Contains("Expected USD", StringComparison.OrdinalIgnoreCase) ||
            jsonCombined.Contains("Actual USD", StringComparison.OrdinalIgnoreCase) ||
            jsonCombined.Contains("Expected CRC", StringComparison.OrdinalIgnoreCase) ||
            jsonCombined.Contains("Actual CRC", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogInformation("✅ UpdateTask - JSON contiene campos Expected/Actual USD/CRC");
        }

        // Actualizar el task existente
        task.UserId = dto.UserId;
        task.Tasks = tasksJson;
        task.CompletedTasks = completedTasksJson;
        task.UpdatedAt = DateTime.UtcNow;

        _context.Entry(task).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("✅ UpdateTask - Task actualizado exitosamente");
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TaskExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    /// <summary>
    /// Procesa los TaskItems para asegurar que todos los campos, incluyendo los de AdditionalProperties, se preserven
    /// También maneja campos con espacios como "Expected USD" desde AdditionalProperties
    /// </summary>
    private List<TaskItemDto> ProcessTaskItems(List<TaskItemDto> items)
    {
        var processed = new List<TaskItemDto>();
        
        foreach (var item in items)
        {
            // Primero intentar obtener valores de las propiedades normales
            var processedItem = new TaskItemDto
            {
                Id = item.Id,
                Title = item.Title,
                Description = item.Description,
                ExpectedUSD = item.ExpectedUSD,
                ActualUSD = item.ActualUSD,
                ExpectedCRC = item.ExpectedCRC,
                ActualCRC = item.ActualCRC,
                AdditionalProperties = new Dictionary<string, object>()
            };
            
            // Si hay AdditionalProperties, procesarlos para preservar todos los campos
            if (item.AdditionalProperties != null)
            {
                foreach (var prop in item.AdditionalProperties)
                {
                    // Si el campo viene con espacios (como "Expected USD"), preservarlo
                    if (prop.Key.Contains("Expected USD", StringComparison.OrdinalIgnoreCase) || 
                        prop.Key.Contains("Actual USD", StringComparison.OrdinalIgnoreCase) ||
                        prop.Key.Contains("Expected CRC", StringComparison.OrdinalIgnoreCase) ||
                        prop.Key.Contains("Actual CRC", StringComparison.OrdinalIgnoreCase))
                    {
                        // Normalizar a camelCase para las propiedades conocidas, pero preservar el valor original
                        if (prop.Key.Contains("Expected USD", StringComparison.OrdinalIgnoreCase) && processedItem.ExpectedUSD == null)
                        {
                            if (decimal.TryParse(prop.Value?.ToString(), out var expectedUsd))
                                processedItem.ExpectedUSD = expectedUsd;
                        }
                        else if (prop.Key.Contains("Actual USD", StringComparison.OrdinalIgnoreCase) && processedItem.ActualUSD == null)
                        {
                            if (decimal.TryParse(prop.Value?.ToString(), out var actualUsd))
                                processedItem.ActualUSD = actualUsd;
                        }
                        else if (prop.Key.Contains("Expected CRC", StringComparison.OrdinalIgnoreCase) && processedItem.ExpectedCRC == null)
                        {
                            if (decimal.TryParse(prop.Value?.ToString(), out var expectedCrc))
                                processedItem.ExpectedCRC = expectedCrc;
                        }
                        else if (prop.Key.Contains("Actual CRC", StringComparison.OrdinalIgnoreCase) && processedItem.ActualCRC == null)
                        {
                            if (decimal.TryParse(prop.Value?.ToString(), out var actualCrc))
                                processedItem.ActualCRC = actualCrc;
                        }
                    }
                    
                    // Preservar todos los campos adicionales para que se guarden en el JSON
                    if (processedItem.AdditionalProperties != null && prop.Value != null)
                    {
                        processedItem.AdditionalProperties[prop.Key] = prop.Value;
                    }
                }
            }
            
            processed.Add(processedItem);
        }
        
        return processed;
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
        {
            return NotFound();
        }

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool TaskExists(Guid id)
    {
        return _context.Tasks.Any(e => e.Id == id);
    }
}

