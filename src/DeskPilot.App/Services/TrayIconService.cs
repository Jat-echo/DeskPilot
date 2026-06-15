using System.Windows;
using System.Windows.Controls;
using Hardcodet.Wpf.TaskbarNotification;

namespace DeskPilot.App.Services;

public class TrayIconService : IDisposable
{
    private TaskbarIcon? _trayIcon;
    private readonly Window _mainWindow;
    private readonly Window _widgetWindow;

    public TrayIconService(Window mainWindow, Window widgetWindow)
    {
        _mainWindow = mainWindow;
        _widgetWindow = widgetWindow;
    }

    public void Initialize()
    {
        _trayIcon = new TaskbarIcon
        {
            ToolTipText = "DeskPilot"
        };

        var contextMenu = new ContextMenu();

        var openItem = new MenuItem { Header = "打开 DeskPilot" };
        openItem.Click += (s, e) => ShowMainWindow();

        var showWidgetItem = new MenuItem { Header = "显示桌面组件" };
        showWidgetItem.Click += (s, e) => ShowWidget();

        var hideWidgetItem = new MenuItem { Header = "隐藏桌面组件" };
        hideWidgetItem.Click += (s, e) => HideWidget();

        var organizeItem = new MenuItem { Header = "整理桌面" };
        organizeItem.Click += (s, e) => OrganizeDesktop();

        var refreshItem = new MenuItem { Header = "刷新飞书数据" };
        refreshItem.Click += (s, e) => RefreshData();

        var exitItem = new MenuItem { Header = "退出程序" };
        exitItem.Click += (s, e) => ExitApplication();

        contextMenu.Items.Add(openItem);
        contextMenu.Items.Add(showWidgetItem);
        contextMenu.Items.Add(new Separator());
        contextMenu.Items.Add(organizeItem);
        contextMenu.Items.Add(refreshItem);
        contextMenu.Items.Add(new Separator());
        contextMenu.Items.Add(exitItem);

        _trayIcon.ContextMenu = contextMenu;
        _trayIcon.TrayMouseDoubleClick += (s, e) => ShowMainWindow();
    }

    public void ShowMainWindow()
    {
        _mainWindow.Show();
        _mainWindow.WindowState = WindowState.Normal;
        _mainWindow.Activate();
    }

    public void ShowWidget()
    {
        _widgetWindow.Show();
    }

    public void HideWidget()
    {
        _widgetWindow.Hide();
    }

    public void OrganizeDesktop()
    {
        ShowMainWindow();
    }

    public void RefreshData()
    {
        // TODO: Trigger refresh
    }

    public void ExitApplication()
    {
        _trayIcon?.Dispose();
        Application.Current.Shutdown();
    }

    public void Dispose()
    {
        _trayIcon?.Dispose();
    }
}
