using DeskPilot.Core.Models;

namespace DeskPilot.Core.Interfaces;

public interface ILarkCliService
{
    Task<bool> IsInstalledAsync();
    Task<string?> GetVersionAsync();
    Task<bool> IsLoggedInAsync();
    Task<string?> GetLoginUrlAsync();
    Task<bool> LoginAsync();
    Task<bool> LogoutAsync();
    Task<List<TaskItem>> GetTasksAsync(DateTime? from = null, DateTime? to = null);
    Task<bool> CreateTaskAsync(string title, DateTime? dueTime = null);
    Task<bool> CompleteTaskAsync(string taskGuid);
    Task<List<CalendarEvent>> GetCalendarEventsAsync(DateTime from, DateTime to);
    Task<bool> CheckUpdateAsync();
}
