using System.Text.Json;
using DeskPilot.Core.Interfaces;
using DeskPilot.Core.Models;

namespace DeskPilot.Infrastructure.Services;

public class SettingsService : ISettingsService
{
    private static readonly string SettingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "DeskPilot",
        "settings.json");

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    private AppSettings? _cachedSettings;

    public AppSettings Load()
    {
        if (_cachedSettings != null)
            return _cachedSettings;

        if (!File.Exists(SettingsPath))
        {
            _cachedSettings = new AppSettings();
            return _cachedSettings;
        }

        try
        {
            var json = File.ReadAllText(SettingsPath);
            if (string.IsNullOrWhiteSpace(json))
            {
                _cachedSettings = new AppSettings();
                return _cachedSettings;
            }
            _cachedSettings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to load settings: {ex.Message}");
            _cachedSettings = new AppSettings();
        }

        return _cachedSettings;
    }

    public void Save(AppSettings settings)
    {
        _cachedSettings = settings;

        var dir = Path.GetDirectoryName(SettingsPath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        var json = JsonSerializer.Serialize(settings, JsonOptions);
        File.WriteAllText(SettingsPath, json);
    }

    public void UpdateAiSettings(AiProviderSettings settings)
    {
        var current = Load();
        current.AiProvider = settings;
        Save(current);
    }

    public void UpdateTheme(ThemeMode mode)
    {
        var current = Load();
        current.Theme = mode;
        Save(current);
    }

    public void UpdateAutoStart(bool enabled)
    {
        var current = Load();
        current.AutoStart = enabled;
        Save(current);
    }
}
