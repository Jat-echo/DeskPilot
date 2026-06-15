using System.Windows;
using System.Windows.Controls;
using DeskPilot.App.Views;

namespace DeskPilot.App;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void NavDashboard_Checked(object sender, RoutedEventArgs e)
    {
        if (ContentFrame == null) return;
        if (ContentFrame.Content is DashboardView) return;
        ContentFrame.Navigate(new DashboardView());
    }

    private void NavOrganize_Checked(object sender, RoutedEventArgs e)
    {
        if (ContentFrame == null) return;
        if (ContentFrame.Content is OrganizeView) return;
        ContentFrame.Navigate(new OrganizeView());
    }

    private void NavAiOrganize_Checked(object sender, RoutedEventArgs e)
    {
        if (ContentFrame == null) return;
        if (ContentFrame.Content is AiOrganizeView) return;
        ContentFrame.Navigate(new AiOrganizeView());
    }

    private void NavLog_Checked(object sender, RoutedEventArgs e)
    {
        if (ContentFrame == null) return;
        if (ContentFrame.Content is LogView) return;
        ContentFrame.Navigate(new LogView());
    }

    private void NavTasks_Checked(object sender, RoutedEventArgs e)
    {
        if (ContentFrame == null) return;
        if (ContentFrame.Content is TaskView) return;
        ContentFrame.Navigate(new TaskView());
    }

    private void NavCalendar_Checked(object sender, RoutedEventArgs e)
    {
        if (ContentFrame == null) return;
        if (ContentFrame.Content is CalendarView) return;
        ContentFrame.Navigate(new CalendarView());
    }

    private void NavSettings_Checked(object sender, RoutedEventArgs e)
    {
        if (ContentFrame == null) return;
        if (ContentFrame.Content is SettingsView) return;
        ContentFrame.Navigate(new SettingsView());
    }
}
