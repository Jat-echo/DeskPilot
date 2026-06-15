using System.Windows;
using System.Windows.Controls;
using DeskPilot.App.Views;

namespace DeskPilot.App;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        ContentFrame.Navigate(new DashboardView());
    }

    private void NavDashboard_Checked(object sender, RoutedEventArgs e)
    {
        ContentFrame.Navigate(new DashboardView());
    }

    private void NavOrganize_Checked(object sender, RoutedEventArgs e)
    {
        ContentFrame.Navigate(new OrganizeView());
    }

    private void NavAiOrganize_Checked(object sender, RoutedEventArgs e)
    {
        ContentFrame.Navigate(new AiOrganizeView());
    }

    private void NavLog_Checked(object sender, RoutedEventArgs e)
    {
        ContentFrame.Navigate(new LogView());
    }

    private void NavTasks_Checked(object sender, RoutedEventArgs e)
    {
        ContentFrame.Navigate(new TaskView());
    }

    private void NavCalendar_Checked(object sender, RoutedEventArgs e)
    {
        ContentFrame.Navigate(new CalendarView());
    }

    private void NavSettings_Checked(object sender, RoutedEventArgs e)
    {
        ContentFrame.Navigate(new SettingsView());
    }
}
