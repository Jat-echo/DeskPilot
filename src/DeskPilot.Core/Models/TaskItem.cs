namespace DeskPilot.Core.Models;

public class TaskItem
{
    public string Guid { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime? DueTime { get; set; }
    public bool IsCompleted { get; set; }
    public TaskPriority Priority { get; set; } = TaskPriority.Normal;
}

public enum TaskPriority
{
    Normal,
    Overdue,
    Today,
    Tomorrow,
    Upcoming
}
