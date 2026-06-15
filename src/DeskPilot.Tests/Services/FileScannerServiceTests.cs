using DeskPilot.Core.Services;
using Xunit;

namespace DeskPilot.Tests.Services;

public class FileScannerServiceTests
{
    [Fact]
    public void GetDesktopPath_ReturnsNonEmptyPath()
    {
        var service = new FileScannerService();
        var path = service.GetDesktopPath();

        Assert.False(string.IsNullOrEmpty(path));
        Assert.Contains("Desktop", path);
    }

    [Fact]
    public void GetActualDesktopPath_ReturnsPath()
    {
        var service = new FileScannerService();
        var path = service.GetActualDesktopPath();

        Assert.False(string.IsNullOrEmpty(path));
    }

    [Fact]
    public async Task ScanDesktopAsync_ReturnsFileList()
    {
        var service = new FileScannerService();
        var files = await service.ScanDesktopAsync();

        Assert.NotNull(files);
    }

    [Fact]
    public async Task ScanDesktopAsync_OnlyReturnsFiles()
    {
        var service = new FileScannerService();
        var files = await service.ScanDesktopAsync();

        // Verify no directories are returned
        foreach (var file in files)
        {
            Assert.False(file.IsDirectory);
        }
    }
}
