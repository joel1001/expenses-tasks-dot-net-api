using Tasks.API.DTOs;

namespace Tasks.API.Services;

/// <summary>
/// Servicio para generar ocurrencias de tareas recurrentes
/// </summary>
public class RecurringTaskService
{
    /// <summary>
    /// Genera las ocurrencias futuras de una tarea recurrente
    /// </summary>
    /// <param name="taskItem">Tarea base con fecha/hora inicial</param>
    /// <param name="maxOccurrences">Número máximo de ocurrencias a generar (default: 365 para daily, menos para otras)</param>
    /// <returns>Lista de ocurrencias futuras con sus fechas/horas</returns>
    public static List<TaskOccurrence> GenerateOccurrences(TaskItemDto taskItem, int maxOccurrences = 365)
    {
        var occurrences = new List<TaskOccurrence>();
        
        if (string.IsNullOrEmpty(taskItem.Date) || string.IsNullOrEmpty(taskItem.Time))
        {
            return occurrences; // Sin fecha/hora, no se pueden generar ocurrencias
        }
        
        var frequency = taskItem.Frequency?.ToLower() ?? "once";
        
        // Si es "once", solo retornar la ocurrencia original
        if (frequency == "once")
        {
            var originalDate = DateTime.Parse($"{taskItem.Date}T{taskItem.Time}");
            if (originalDate > DateTime.UtcNow)
            {
                occurrences.Add(new TaskOccurrence
                {
                    Date = taskItem.Date,
                    Time = taskItem.Time,
                    TaskDateTime = originalDate
                });
            }
            return occurrences;
        }
        
        // Parsear fecha/hora inicial
        var startDate = DateTime.Parse($"{taskItem.Date}T{taskItem.Time}");
        var now = DateTime.UtcNow;
        
        // Si la fecha inicial es en el pasado, calcular la próxima ocurrencia
        DateTime currentOccurrence = startDate;
        if (currentOccurrence <= now)
        {
            currentOccurrence = GetNextOccurrence(startDate, frequency, now);
        }
        
        // Generar ocurrencias futuras
        int count = 0;
        while (count < maxOccurrences && currentOccurrence <= now.AddYears(1))
        {
            occurrences.Add(new TaskOccurrence
            {
                Date = currentOccurrence.ToString("yyyy-MM-dd"),
                Time = currentOccurrence.ToString("HH:mm"),
                TaskDateTime = currentOccurrence
            });
            
            currentOccurrence = GetNextOccurrence(startDate, frequency, currentOccurrence);
            count++;
        }
        
        return occurrences;
    }
    
    /// <summary>
    /// Calcula la próxima ocurrencia basada en la frecuencia
    /// </summary>
    private static DateTime GetNextOccurrence(DateTime startDate, string frequency, DateTime fromDate)
    {
        return frequency switch
        {
            "daily" => fromDate.AddDays(1),
            "weekly" => fromDate.AddDays(7),
            "biweekly" => fromDate.AddDays(14),
            "monthly" => fromDate.AddMonths(1),
            _ => fromDate.AddDays(1) // Default a daily si no se reconoce
        };
    }
}

/// <summary>
/// Representa una ocurrencia de una tarea recurrente
/// </summary>
public class TaskOccurrence
{
    public string Date { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public DateTime TaskDateTime { get; set; }
}
