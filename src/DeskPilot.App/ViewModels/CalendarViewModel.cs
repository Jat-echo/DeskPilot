using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DeskPilot.Core.Interfaces;
using DeskPilot.Core.Models;
using DeskPilot.Infrastructure.Services;

namespace DeskPilot.App.ViewModels;

public partial class CalendarViewModel : ObservableObject
{
    private readonly ILarkCliService _larkCliService;

    [ObservableProperty]
    private List<CalendarEvent> _events = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private DateTime _selectedDate = DateTime.Today;

    public CalendarViewModel()
    {
        _larkCliService = new LarkCliService();
        _ = LoadEventsAsync();
    }

    private async Task LoadEventsAsync()
    {
        IsLoading = true;
        Events = await _larkCliService.GetCalendarEventsAsync(
            DateTime.Today,
            DateTime.Today.AddDays(7));
        IsLoading = false;
    }

    [RelayCommand]
    private async Task Refresh()
    {
        await LoadEventsAsync();
    }
}
