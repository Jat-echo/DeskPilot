using System.Windows;
using System.Windows.Controls;
using DeskPilot.Core.Interfaces;
using DeskPilot.Core.Models;
using DeskPilot.Infrastructure.Services;

namespace DeskPilot.App.Views;

public partial class SettingsView : Page
{
    private readonly ISettingsService _settingsService;

    public SettingsView()
    {
        InitializeComponent();
        _settingsService = new SettingsService();
        LoadSettings();
    }

    private void LoadSettings()
    {
        var settings = _settingsService.Load();
        AiEnabledCheck.IsChecked = settings.AiProvider.Enabled;
        AiBaseUrlBox.Text = settings.AiProvider.BaseUrl;
        AiApiKeyBox.Password = settings.AiProvider.ApiKey;
        AiModelBox.Text = settings.AiProvider.ModelName;

        ThemeLight.IsChecked = settings.Theme == ThemeMode.Light;
        ThemeDark.IsChecked = settings.Theme == ThemeMode.Dark;
        ThemeSystem.IsChecked = settings.Theme == ThemeMode.System;

        AutoStartCheckBox.IsChecked = settings.AutoStart;
    }

    private void SaveAiButton_Click(object sender, RoutedEventArgs e)
    {
        var settings = new AppSettings
        {
            AiProvider = new AiProviderSettings
            {
                Enabled = AiEnabledCheck.IsChecked == true,
                BaseUrl = AiBaseUrlBox.Text,
                ApiKey = AiApiKeyBox.Password,
                ModelName = AiModelBox.Text,
                Temperature = 0.7,
                MaxTokens = 2000,
                TimeoutSeconds = 30
            },
            Theme = ThemeLight.IsChecked == true ? ThemeMode.Light :
                    ThemeDark.IsChecked == true ? ThemeMode.Dark : ThemeMode.System,
            AutoStart = AutoStartCheckBox.IsChecked == true,
            TaskRefreshMinutes = 10,
            CalendarRefreshMinutes = 10
        };

        _settingsService.Save(settings);
        MessageBox.Show("设置已保存", "DeskPilot", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void ConnectLarkButton_Click(object sender, RoutedEventArgs e)
    {
        LarkStatusText.Text = "正在打开授权页面...";
        // Lark connection would be handled by LarkCliService
        MessageBox.Show("请在浏览器中完成飞书授权", "连接飞书", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void ThemeLight_Checked(object sender, RoutedEventArgs e)
    {
        ApplyTheme(ThemeMode.Light);
    }

    private void ThemeDark_Checked(object sender, RoutedEventArgs e)
    {
        ApplyTheme(ThemeMode.Dark);
    }

    private void ThemeSystem_Checked(object sender, RoutedEventArgs e)
    {
        ApplyTheme(ThemeMode.System);
    }

    private void ApplyTheme(ThemeMode mode)
    {
        var settings = _settingsService.Load();
        settings.Theme = mode;
        _settingsService.Save(settings);

        // Theme application would be handled by ThemeService in App.xaml.cs
    }

    private void AutoStartCheckBox_Changed(object sender, RoutedEventArgs e)
    {
        var settings = _settingsService.Load();
        settings.AutoStart = AutoStartCheckBox.IsChecked == true;
        _settingsService.Save(settings);
    }
}
