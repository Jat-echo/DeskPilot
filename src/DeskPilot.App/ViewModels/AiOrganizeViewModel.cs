using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DeskPilot.Core.Interfaces;
using DeskPilot.Core.Models;
using DeskPilot.Core.Services;
using DeskPilot.Infrastructure.Services;

namespace DeskPilot.App.ViewModels;

public partial class AiOrganizeViewModel : ObservableObject
{
    private readonly IFileScannerService _scannerService;
    private readonly IAiPlanningService _aiPlanningService;
    private readonly ISettingsService _settingsService;

    [ObservableProperty]
    private string _userRequest = string.Empty;

    [ObservableProperty]
    private bool _isProcessing;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private OrganizePlan? _plan;

    public AiOrganizeViewModel()
    {
        _scannerService = new FileScannerService();
        _aiPlanningService = new AiPlanningService();
        _settingsService = new SettingsService();
    }

    [RelayCommand]
    private async Task GeneratePlan()
    {
        if (string.IsNullOrWhiteSpace(UserRequest))
            return;

        IsProcessing = true;
        ErrorMessage = null;

        try
        {
            var files = await _scannerService.ScanDesktopAsync();
            var settings = _settingsService.Load();
            var result = await _aiPlanningService.SendRequestAsync(files, UserRequest, settings.AiProvider);

            if (result.Success && result.Plan != null)
            {
                Plan = result.Plan;
            }
            else
            {
                ErrorMessage = result.ErrorMessage ?? "AI 整理失败";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsProcessing = false;
        }
    }
}
