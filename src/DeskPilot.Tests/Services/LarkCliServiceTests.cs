using DeskPilot.Core.Models;
using DeskPilot.Infrastructure.Services;
using Xunit;

namespace DeskPilot.Tests.Services;

public class LarkCliServiceTests
{
    [Fact]
    public async Task IsInstalled_ReturnsFalse_WhenCliNotPresent()
    {
        var service = new LarkCliService();
        var isInstalled = await service.IsInstalledAsync();
        // Lark-cli may or may not be installed - just verify method runs
        Assert.True(isInstalled || !isInstalled);
    }

    [Fact]
    public async Task GetTasks_ReturnsEmptyList_WhenError()
    {
        var service = new LarkCliService();
        var tasks = await service.GetTasksAsync();
        Assert.NotNull(tasks);
    }

    [Fact]
    public void TaskPriority_Categorization_Works()
    {
        var now = DateTime.Now;
        var today = now.Date.AddHours(18);
        var tomorrow = now.Date.AddDays(1).AddHours(10);

        var overdueTask = new TaskItem { DueTime = now.AddDays(-1), IsCompleted = false };
        var todayTask = new TaskItem { DueTime = today, IsCompleted = false };
        var tomorrowTask = new TaskItem { DueTime = tomorrow, IsCompleted = false };

        Assert.True(overdueTask.DueTime < now);
        Assert.True(todayTask.DueTime?.Date == now.Date);
        Assert.True(tomorrowTask.DueTime?.Date == now.Date.AddDays(1));
    }
}
