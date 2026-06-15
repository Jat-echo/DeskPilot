using System.Windows;
using System.Windows.Controls;
using DeskPilot.App.Views;

namespace DeskPilot.App;

public partial class MainWindow : Window
{
    private Type? _currentPageType;

    public MainWindow()
    {
        InitializeComponent();
        _currentPageType = typeof(DashboardView);
    }

    private void NavigateIfNeeded(Type pageType, object page)
    {
        if (_currentPageType == pageType) return;
        _currentPageType = pageType;
        ContentFrame.Navigate(page);
    }

    private void NavDashboard_Checked(object sender, RoutedEventArgs e)
    {
        if (ContentFrame == null) return;
        NavigateIfNeeded(typeof(DashboardView), new DashboardView());
    }

    private void NavOrganize_Checked(object sender, RoutedEventArgs e)
    {
        if (ContentFrame == null) return;
        NavigateIfNeeded(typeof(OrganizeView), new OrganizeView());
    }

    private void NavAiOrganize_Checked(object sender, RoutedEventArgs e)
    {
        if (ContentFrame == null) return;
        NavigateIfNeeded(typeof(AiOrganizeView), new AiOrganizeView());
    }

    private void NavLog_Checked(object sender, RoutedEventArgs e)
    {
        if (ContentFrame == null) return;
        NavigateIfNeeded(typeof(LogView), new LogView());
    }

    private void NavTasks_Checked(object sender, RoutedEventArgs e)
    {
        if (ContentFrame == null) return;
        NavigateIfNeeded(typeof(TaskView), new TaskView());
    }

    private void NavCalendar_Checked(object sender, RoutedEventArgs e)
    {
        if (ContentFrame == null) return;
        NavigateIfNeeded(typeof(CalendarView), new CalendarView());
    }

    private void NavSettings_Checked(object sender, RoutedEventArgs e)
    {
        if (ContentFrame == null) return;
        NavigateIfNeeded(typeof(SettingsView), new SettingsView());
    }
}
