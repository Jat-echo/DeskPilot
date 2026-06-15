using CommunityToolkit.Mvvm.ComponentModel;
using DeskPilot.Core.Interfaces;
using DeskPilot.Core.Models;
using DeskPilot.Core.Services;

namespace DeskPilot.App.ViewModels;

public partial class OrganizeViewModel : ObservableObject
{
    private readonly IFileScannerService _scannerService;
    private readonly IOrganizePlanService _planService;

    [ObservableProperty]
    private List<DesktopFile> _files = new();

    [ObservableProperty]
    private int _totalFiles;

    [ObservableProperty]
    private bool _isScanning;

    public OrganizeViewModel()
    {
        _scannerService = new FileScannerService();
        _planService = new RuleBasedPlanService();

        _ = ScanDesktopAsync();
    }

    private async Task ScanDesktopAsync()
    {
        IsScanning = true;
        Files = await _scannerService.ScanDesktopAsync();
        TotalFiles = Files.Count;
        IsScanning = false;
    }
}
