using System.Windows;
using DeskPilot.App.Services;
using DeskPilot.App.Views.Components;

namespace DeskPilot.App;

public partial class App : Application
{
    private TrayIconService? _trayIconService;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var mainWindow = new MainWindow();
        var widgetWindow = new DesktopWidgetWindow();

        _trayIconService = new TrayIconService(mainWindow, widgetWindow);
        _trayIconService.Initialize();

        widgetWindow.Show();
        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _trayIconService?.Dispose();
        base.OnExit(e);
    }
}
