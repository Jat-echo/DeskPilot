using DeskPilot.Core.Models;

namespace DeskPilot.Core.Interfaces;

public interface IFileScannerService
{
    Task<List<DesktopFile>> ScanDesktopAsync();
    string GetDesktopPath();
    string GetActualDesktopPath();
}
