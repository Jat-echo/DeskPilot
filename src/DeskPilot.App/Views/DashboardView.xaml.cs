using System.Windows;
using System.Windows.Controls;
using DeskPilot.Core.Models;
using DeskPilot.Core.Services;
using DeskPilot.Infrastructure.Services;

namespace DeskPilot.App.Views;

public partial class DashboardView : Page
{
    public DashboardView()
    {
        InitializeComponent();
        Loaded += DashboardView_Loaded;
    }

    private async void DashboardView_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            var scanner = new FileScannerService();
            var files = await scanner.ScanDesktopAsync();

            var imageCount = files.Count(f => f.Category == FileCategory.Image);
            var docCount = files.Count(f => f.Category == FileCategory.Document);
            FileCountText.Text = $"检测到 {files.Count} 个文件（图片 {imageCount}，文档 {docCount}）";

            var lark = new LarkCliService();
            var isConnected = await lark.IsLoggedInAsync();

            if (isConnected)
            {
                var tasks = await lark.GetTasksAsync(DateTime.Now, DateTime.Now.AddDays(14));
                var overdue = tasks.Count(t => !t.IsCompleted && t.DueTime < DateTime.Now);
                var today = tasks.Count(t => !t.IsCompleted && t.DueTime?.Date == DateTime.Today);
                OverdueText.Text = $"{overdue} 个逾期";
                TodayText.Text = $"{today} 个今天到期";
            }
            else
            {
                OverdueText.Text = "未连接飞书";
                TodayText.Text = "请在设置中连接飞书";
            }
        }
        catch
        {
            FileCountText.Text = "加载失败";
            OverdueText.Text = "";
            TodayText.Text = "";
        }
    }
}
