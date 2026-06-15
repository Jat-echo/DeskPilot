namespace DeskPilot.Core.Models;

public class CalendarEvent
{
    public string Guid { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? Location { get; set; }
    public string? MeetingUrl { get; set; }
    public int AttendeeCount { get; set; }
    public EventStatus Status { get; set; } = EventStatus.Normal;
}

public enum EventStatus
{
    Normal,
    InProgress,
    EndingSoon,
    Ended
}
