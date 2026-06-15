using DeskPilot.Core.Interfaces;
using DeskPilot.Infrastructure.Services;
using Xunit;

namespace DeskPilot.Tests.Services;

public class SettingsServiceTests
{
    [Fact]
    public void Load_ReturnsDefaultSettings_WhenFileNotExists()
    {
        var service = new SettingsService();
        var settings = service.Load();

        Assert.NotNull(settings);
        Assert.NotNull(settings.AiProvider);
        Assert.Equal(10, settings.TaskRefreshMinutes);
        Assert.Equal(10, settings.CalendarRefreshMinutes);
    }

    [Fact]
    public void ThemeMode_HasExpectedValues()
    {
        Assert.Equal(0, (int)ThemeMode.Light);
        Assert.Equal(1, (int)ThemeMode.Dark);
        Assert.Equal(2, (int)ThemeMode.System);
    }
}
