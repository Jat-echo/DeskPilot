using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DeskPilot.Core.Interfaces;
using DeskPilot.Core.Models;
using DeskPilot.Infrastructure.Services;

namespace DeskPilot.App.ViewModels;

public partial class TaskViewModel : ObservableObject
{
    private readonly ILarkCliService _larkCliService;

    [ObservableProperty]
    private List<TaskItem> _tasks = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _newTaskTitle = string.Empty;

    public TaskViewModel()
    {
        _larkCliService = new LarkCliService();
        _ = LoadTasksAsync();
    }

    private async Task LoadTasksAsync()
    {
        IsLoading = true;
        Tasks = await _larkCliService.GetTasksAsync(DateTime.Now, DateTime.Now.AddDays(14));
        IsLoading = false;
    }

    [RelayCommand]
    private async Task AddTask()
    {
        if (string.IsNullOrWhiteSpace(NewTaskTitle))
            return;

        var success = await _larkCliService.CreateTaskAsync(NewTaskTitle);
        if (success)
        {
            NewTaskTitle = string.Empty;
            await LoadTasksAsync();
        }
    }

    [RelayCommand]
    private async Task CompleteTask(TaskItem task)
    {
        await _larkCliService.CompleteTaskAsync(task.Guid);
        await LoadTasksAsync();
    }

    [RelayCommand]
    private async Task Refresh()
    {
        await LoadTasksAsync();
    }
}
