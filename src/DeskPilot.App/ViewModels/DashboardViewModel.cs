using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DeskPilot.Core.Interfaces;
using DeskPilot.Core.Models;
using DeskPilot.Core.Services;
using DeskPilot.Infrastructure.Services;

namespace DeskPilot.App.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly IFileScannerService _scannerService;
    private readonly ILarkCliService _larkCliService;

    [ObservableProperty]
    private int _totalFiles;

    [ObservableProperty]
    private int _imageCount;

    [ObservableProperty]
    private int _documentCount;

    [ObservableProperty]
    private int _overdueTaskCount;

    [ObservableProperty]
    private int _todayTaskCount;

    [ObservableProperty]
    private string _lastOrganizeTime = "尚未整理";

    [ObservableProperty]
    private bool _isLarkConnected;

    [ObservableProperty]
    private string _aiRequest = string.Empty;

    public DashboardViewModel()
    {
        _scannerService = new FileScannerService();
        _larkCliService = new LarkCliService();

        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            var files = await _scannerService.ScanDesktopAsync();
            TotalFiles = files.Count;
            ImageCount = files.Count(f => f.Category == FileCategory.Image);
            DocumentCount = files.Count(f => f.Category == FileCategory.Document);

            IsLarkConnected = await _larkCliService.IsLoggedInAsync();

            if (IsLarkConnected)
            {
                var tasks = await _larkCliService.GetTasksAsync(DateTime.Now, DateTime.Now.AddDays(14));
                OverdueTaskCount = tasks.Count(t => !t.IsCompleted && t.DueTime < DateTime.Now);
                TodayTaskCount = tasks.Count(t => !t.IsCompleted && t.DueTime?.Date == DateTime.Today);
            }
        }
        catch
        {
            IsLarkConnected = false;
        }
    }

    [RelayCommand]
    private async Task QuickOrganize()
    {
        // Navigate to preview with rule-based plan
    }

    [RelayCommand]
    private async Task SendAiRequest()
    {
        if (string.IsNullOrWhiteSpace(AiRequest))
            return;
    }
}
