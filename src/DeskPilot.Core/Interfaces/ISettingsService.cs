using DeskPilot.Core.Models;

namespace DeskPilot.Core.Interfaces;

public interface ISettingsService
{
    AppSettings Load();
    void Save(AppSettings settings);
    void UpdateAiSettings(AiProviderSettings settings);
    void UpdateTheme(ThemeMode mode);
    void UpdateAutoStart(bool enabled);
}

public class AppSettings
{
    public AiProviderSettings AiProvider { get; set; } = new();
    public ThemeMode Theme { get; set; } = ThemeMode.System;
    public bool AutoStart { get; set; }
    public int TaskRefreshMinutes { get; set; } = 10;
    public int CalendarRefreshMinutes { get; set; } = 10;
}

public enum ThemeMode
{
    Light,
    Dark,
    System
}
