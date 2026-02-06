using Notifications.API.DTOs;

namespace Notifications.API.Services;

/// <summary>
/// Helper para generar ocurrencias de tareas recurrentes (versión local para Notifications.API)
/// </summary>
public static class RecurringTaskHelper
{
    /// <summary>
    /// Genera las ocurrencias futuras de una tarea recurrente
    /// </summary>
    public static List<TaskOccurrence> GenerateOccurrences(TaskItemDto taskItem, int maxOccurrences = 365)
    {
        var occurrences = new List<TaskOccurrence>();
        
        if (string.IsNullOrEmpty(taskItem.Date) || string.IsNullOrEmpty(taskItem.Time))
        {
            return occurrences;
        }
        
        var frequency = taskItem.Frequency?.ToLower() ?? "once";
        
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
        
        var startDate = DateTime.Parse($"{taskItem.Date}T{taskItem.Time}");
        var now = DateTime.UtcNow;
        
        DateTime currentOccurrence = startDate;
        if (currentOccurrence <= now)
        {
            currentOccurrence = GetNextOccurrence(startDate, frequency, now);
        }
        
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
    
    private static DateTime GetNextOccurrence(DateTime startDate, string frequency, DateTime fromDate)
    {
        if (fromDate < startDate)
        {
            return startDate;
        }
        
        var nextDate = fromDate;
        switch (frequency)
        {
            case "daily":
                nextDate = nextDate.AddDays(1);
                break;
            case "weekly":
                nextDate = nextDate.AddDays(7);
                break;
            case "biweekly":
                nextDate = nextDate.AddDays(14);
                break;
            case "monthly":
                nextDate = nextDate.AddMonths(1);
                break;
        }
        
        return nextDate;
    }
}

public class TaskOccurrence
{
    public string Date { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public DateTime TaskDateTime { get; set; }
}
