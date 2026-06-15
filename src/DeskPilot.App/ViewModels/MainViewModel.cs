using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DeskPilot.Core.Interfaces;
using DeskPilot.Infrastructure.Services;

namespace DeskPilot.App.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly NavigationService _navigationService;
    private readonly ISettingsService _settingsService;

    [ObservableProperty]
    private ObservableObject? _currentViewModel;

    [ObservableProperty]
    private string _currentPage = "首页";

    [ObservableProperty]
    private bool _isLoggedIn;

    [ObservableProperty]
    private string _connectionStatus = "未连接";

    public MainViewModel()
    {
        _navigationService = new NavigationService();
        _settingsService = new SettingsService();

        NavigateToDashboard();
    }

    [RelayCommand]
    private void NavigateToDashboard()
    {
        CurrentViewModel = new DashboardViewModel();
        CurrentPage = "首页";
    }

    [RelayCommand]
    private void NavigateToOrganize()
    {
        CurrentViewModel = new OrganizeViewModel();
        CurrentPage = "桌面整理";
    }

    [RelayCommand]
    private void NavigateToAiOrganize()
    {
        CurrentViewModel = new AiOrganizeViewModel();
        CurrentPage = "AI 整理";
    }

    [RelayCommand]
    private void NavigateToLog()
    {
        CurrentViewModel = new LogViewModel();
        CurrentPage = "整理日志";
    }

    [RelayCommand]
    private void NavigateToTasks()
    {
        CurrentViewModel = new TaskViewModel();
        CurrentPage = "飞书待办";
    }

    [RelayCommand]
    private void NavigateToCalendar()
    {
        CurrentViewModel = new CalendarViewModel();
        CurrentPage = "飞书日程";
    }

    [RelayCommand]
    private void NavigateToSettings()
    {
        CurrentViewModel = new SettingsViewModel();
        CurrentPage = "设置";
    }
}
