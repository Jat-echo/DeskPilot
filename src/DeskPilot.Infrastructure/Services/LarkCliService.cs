using System.Diagnostics;
using System.Text.Json;
using DeskPilot.Core.Interfaces;
using DeskPilot.Core.Models;

namespace DeskPilot.Infrastructure.Services;

public class LarkCliService : ILarkCliService
{
    private const string LarkCliPath = "lark-cli";
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true
    };

    public async Task<bool> IsInstalledAsync()
    {
        try
        {
            var result = await RunCommandAsync("--version");
            return result.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }

    public async Task<string?> GetVersionAsync()
    {
        try
        {
            var result = await RunCommandAsync("--version");
            if (result.ExitCode == 0)
            {
                return result.StdOut.Trim();
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> IsLoggedInAsync()
    {
        try
        {
            var result = await RunCommandAsync("auth status");
            return result.ExitCode == 0 && result.StdOut.Contains("logged in", StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    public async Task<string?> GetLoginUrlAsync()
    {
        try
        {
            var result = await RunCommandAsync("auth login-url");
            if (result.ExitCode == 0)
            {
                using var doc = JsonDocument.Parse(result.StdOut);
                if (doc.RootElement.TryGetProperty("data", out var data) &&
                    data.TryGetProperty("url", out var url))
                {
                    return url.GetString();
                }
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> LoginAsync()
    {
        try
        {
            var result = await RunCommandAsync("auth login");
            return result.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> LogoutAsync()
    {
        try
        {
            var result = await RunCommandAsync("auth logout");
            return result.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<TaskItem>> GetTasksAsync(DateTime? from = null, DateTime? to = null)
    {
        try
        {
            var args = "task list --json";
            if (from.HasValue)
                args += $" --from {from.Value:yyyy-MM-dd}";
            if (to.HasValue)
                args += $" --to {to.Value:yyyy-MM-dd}";

            var result = await RunCommandAsync(args);
            if (result.ExitCode != 0)
                return new List<TaskItem>();

            return ParseTasksFromJson(result.StdOut);
        }
        catch
        {
            return new List<TaskItem>();
        }
    }

    public async Task<bool> CreateTaskAsync(string title, DateTime? dueTime = null)
    {
        try
        {
            var args = $"task create --title \"{EscapeArg(title)}\"";
            if (dueTime.HasValue)
                args += $" --due {dueTime.Value:yyyy-MM-dd HH:mm}";

            var result = await RunCommandAsync(args);
            return result.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> CompleteTaskAsync(string taskGuid)
    {
        try
        {
            var result = await RunCommandAsync($"task complete {taskGuid}");
            return result.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<CalendarEvent>> GetCalendarEventsAsync(DateTime from, DateTime to)
    {
        try
        {
            var result = await RunCommandAsync(
                $"calendar list --from {from:yyyy-MM-dd} --to {to:yyyy-MM-dd} --json");

            if (result.ExitCode != 0)
                return new List<CalendarEvent>();

            return ParseCalendarEventsFromJson(result.StdOut);
        }
        catch
        {
            return new List<CalendarEvent>();
        }
    }

    public async Task<bool> CheckUpdateAsync()
    {
        try
        {
            var result = await RunCommandAsync("update check");
            return result.ExitCode == 0 && result.StdOut.Contains("update available", StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    private async Task<CommandResult> RunCommandAsync(string arguments, TimeSpan? timeout = null)
    {
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = LarkCliPath,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };

        process.Start();
        var timeoutValue = timeout ?? DefaultTimeout;

        using var cts = new CancellationTokenSource(timeoutValue);
        try
        {
            await process.WaitForExitAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            process.Kill(true);
            return new CommandResult { ExitCode = -1, StdOut = "", StdErr = "Command timed out" };
        }

        var stdOut = await process.StandardOutput.ReadToEndAsync();
        var stdErr = await process.StandardError.ReadToEndAsync();

        return new CommandResult
        {
            ExitCode = process.ExitCode,
            StdOut = stdOut,
            StdErr = stdErr
        };
    }

    private List<TaskItem> ParseTasksFromJson(string json)
    {
        var tasks = new List<TaskItem>();
        try
        {
            using var doc = JsonDocument.Parse(json);
            var data = doc.RootElement;
            if (data.TryGetProperty("data", out var taskList))
            {
                foreach (var item in taskList.EnumerateArray())
                {
                    tasks.Add(new TaskItem
                    {
                        Guid = item.GetProperty("guid").GetString() ?? "",
                        Title = item.GetProperty("title").GetString() ?? "",
                        DueTime = item.TryGetProperty("due_time", out var due) ? due.GetDateTime() : null,
                        IsCompleted = item.GetProperty("completed").GetBoolean()
                    });
                }
            }
        }
        catch { }
        return tasks;
    }

    private List<CalendarEvent> ParseCalendarEventsFromJson(string json)
    {
        var events = new List<CalendarEvent>();
        try
        {
            using var doc = JsonDocument.Parse(json);
            var data = doc.RootElement;
            if (data.TryGetProperty("data", out var eventList))
            {
                foreach (var item in eventList.EnumerateArray())
                {
                    events.Add(new CalendarEvent
                    {
                        Guid = item.GetProperty("event_id").GetString() ?? "",
                        Title = item.GetProperty("summary").GetString() ?? "",
                        StartTime = item.GetProperty("start_time").GetDateTime(),
                        EndTime = item.GetProperty("end_time").GetDateTime(),
                        Location = item.TryGetProperty("location", out var loc) ? loc.GetString() : null,
                        MeetingUrl = item.TryGetProperty("meeting_url", out var url) ? url.GetString() : null,
                        AttendeeCount = item.TryGetProperty("attendee_count", out var count) ? count.GetInt32() : 0
                    });
                }
            }
        }
        catch { }
        return events;
    }

    private static string EscapeArg(string arg)
    {
        return arg.Replace("\"", "\\\"");
    }

    private class CommandResult
    {
        public int ExitCode { get; set; }
        public string StdOut { get; set; } = "";
        public string StdErr { get; set; } = "";
    }
}
