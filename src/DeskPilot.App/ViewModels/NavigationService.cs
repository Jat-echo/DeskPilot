using CommunityToolkit.Mvvm.ComponentModel;

namespace DeskPilot.App.ViewModels;

public class NavigationService : ObservableObject
{
    private ObservableObject? _currentViewModel;

    public ObservableObject? CurrentViewModel
    {
        get => _currentViewModel;
        set => SetProperty(ref _currentViewModel, value);
    }

    public void NavigateTo<T>() where T : ObservableObject, new()
    {
        CurrentViewModel = new T();
    }

    public void NavigateTo(ObservableObject viewModel)
    {
        CurrentViewModel = viewModel;
    }
}
