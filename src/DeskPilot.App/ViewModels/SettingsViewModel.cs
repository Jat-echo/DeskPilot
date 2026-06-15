using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DeskPilot.Core.Interfaces;
using DeskPilot.Core.Models;
using DeskPilot.Infrastructure.Services;

namespace DeskPilot.App.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly ISettingsService _settingsService;
    private readonly ILarkCliService _larkCliService;

    [ObservableProperty]
    private bool _aiEnabled;

    [ObservableProperty]
    private string _aiBaseUrl = "https://api.openai.com/v1";

    [ObservableProperty]
    private string _aiApiKey = string.Empty;

    [ObservableProperty]
    private string _aiModelName = "gpt-4o-mini";

    [ObservableProperty]
    private double _aiTemperature = 0.7;

    [ObservableProperty]
    private int _aiMaxTokens = 2000;

    [ObservableProperty]
    private int _aiTimeout = 30;

    [ObservableProperty]
    private ThemeMode _selectedTheme = ThemeMode.System;

    [ObservableProperty]
    private bool _autoStart;

    [ObservableProperty]
    private int _taskRefreshMinutes = 10;

    [ObservableProperty]
    private int _calendarRefreshMinutes = 10;

    [ObservableProperty]
    private bool _isLarkConnected;

    [ObservableProperty]
    private string _larkVersion = string.Empty;

    public SettingsViewModel()
    {
        _settingsService = new SettingsService();
        _larkCliService = new LarkCliService();

        try
        {
            LoadSettings();
            _ = CheckLarkStatusAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"SettingsViewModel init failed: {ex.Message}");
            LoadSettings();
        }
    }

    private void LoadSettings()
    {
        var settings = _settingsService.Load();
        AiEnabled = settings.AiProvider.Enabled;
        AiBaseUrl = settings.AiProvider.BaseUrl;
        AiApiKey = settings.AiProvider.ApiKey;
        AiModelName = settings.AiProvider.ModelName;
        AiTemperature = settings.AiProvider.Temperature;
        AiMaxTokens = settings.AiProvider.MaxTokens;
        AiTimeout = settings.AiProvider.TimeoutSeconds;
        SelectedTheme = settings.Theme;
        AutoStart = settings.AutoStart;
        TaskRefreshMinutes = settings.TaskRefreshMinutes;
        CalendarRefreshMinutes = settings.CalendarRefreshMinutes;
    }

    private async Task CheckLarkStatusAsync()
    {
        try
        {
            IsLarkConnected = await _larkCliService.IsLoggedInAsync();
            LarkVersion = await _larkCliService.GetVersionAsync() ?? "未安装";
        }
        catch
        {
            IsLarkConnected = false;
            LarkVersion = "未安装";
        }
    }

    [RelayCommand]
    private void SaveSettings()
    {
        var settings = new AppSettings
        {
            AiProvider = new AiProviderSettings
            {
                Enabled = AiEnabled,
                BaseUrl = AiBaseUrl,
                ApiKey = AiApiKey,
                ModelName = AiModelName,
                Temperature = AiTemperature,
                MaxTokens = AiMaxTokens,
                TimeoutSeconds = AiTimeout
            },
            Theme = SelectedTheme,
            AutoStart = AutoStart,
            TaskRefreshMinutes = TaskRefreshMinutes,
            CalendarRefreshMinutes = CalendarRefreshMinutes
        };

        _settingsService.Save(settings);
    }

    [RelayCommand]
    private async Task ConnectLark()
    {
        await _larkCliService.LoginAsync();
        await CheckLarkStatusAsync();
    }

    [RelayCommand]
    private async Task DisconnectLark()
    {
        await _larkCliService.LogoutAsync();
        await CheckLarkStatusAsync();
    }
}
