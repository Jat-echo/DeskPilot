using System.Windows;
using System.Windows.Input;

namespace DeskPilot.App.Views.Components;

public partial class DesktopWidgetWindow : Window
{
    private bool _isPinned;

    public DesktopWidgetWindow()
    {
        InitializeComponent();
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (!_isPinned)
        {
            DragMove();
        }
    }

    private void Refresh_Click(object sender, RoutedEventArgs e)
    {
        // TODO: Refresh data
    }

    private void Minimize_Click(object sender, RoutedEventArgs e)
    {
        // TODO: Minimize to collapsed state
    }

    private void Pin_Click(object sender, RoutedEventArgs e)
    {
        _isPinned = !_isPinned;
    }

    private void Hide_Click(object sender, RoutedEventArgs e)
    {
        Hide();
    }

    private void TaskInput_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            // TODO: Create task
            TaskInput.Text = string.Empty;
        }
    }

    private void ShowAllTasks_Click(object sender, MouseButtonEventArgs e)
    {
        // TODO: Navigate to all tasks
    }

    private void ShowAllCalendar_Click(object sender, MouseButtonEventArgs e)
    {
        // TODO: Navigate to calendar
    }

    private void Organize_Click(object sender, RoutedEventArgs e)
    {
        // TODO: Trigger organize
    }

    private void AiOrganize_Click(object sender, RoutedEventArgs e)
    {
        // TODO: Open AI organize dialog
    }

    private void OpenMainWindow_Click(object sender, MouseButtonEventArgs e)
    {
        // TODO: Open main window
    }
}
