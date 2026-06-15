using DeskPilot.Core.Interfaces;
using DeskPilot.Core.Models;

namespace DeskPilot.Core.Services;

public class FileScannerService : IFileScannerService
{
    private static readonly Dictionary<string, FileCategory> ExtensionMap = new(StringComparer.OrdinalIgnoreCase)
    {
        // Images
        { ".jpg", FileCategory.Image }, { ".jpeg", FileCategory.Image }, { ".png", FileCategory.Image },
        { ".webp", FileCategory.Image }, { ".gif", FileCategory.Image }, { ".bmp", FileCategory.Image },
        { ".svg", FileCategory.Image }, { ".heic", FileCategory.Image },
        // Documents
        { ".pdf", FileCategory.Document }, { ".doc", FileCategory.Document }, { ".docx", FileCategory.Document },
        { ".txt", FileCategory.Document }, { ".md", FileCategory.Document }, { ".rtf", FileCategory.Document },
        // Spreadsheets
        { ".xls", FileCategory.Spreadsheet }, { ".xlsx", FileCategory.Spreadsheet },
        { ".csv", FileCategory.Spreadsheet }, { ".tsv", FileCategory.Spreadsheet },
        // Presentations
        { ".ppt", FileCategory.Presentation }, { ".pptx", FileCategory.Presentation }, { ".key", FileCategory.Presentation },
        // Archives
        { ".zip", FileCategory.Archive }, { ".rar", FileCategory.Archive }, { ".7z", FileCategory.Archive },
        { ".tar", FileCategory.Archive }, { ".gz", FileCategory.Archive },
        // Installers
        { ".exe", FileCategory.Installer }, { ".msi", FileCategory.Installer }, { ".dmg", FileCategory.Installer }, { ".pkg", FileCategory.Installer },
        // Code
        { ".cs", FileCategory.Code }, { ".py", FileCategory.Code }, { ".js", FileCategory.Code },
        { ".ts", FileCategory.Code }, { ".jsx", FileCategory.Code }, { ".tsx", FileCategory.Code },
        { ".html", FileCategory.Code }, { ".css", FileCategory.Code }, { ".json", FileCategory.Code },
        { ".xml", FileCategory.Code }, { ".yaml", FileCategory.Code }, { ".yml", FileCategory.Code }, { ".sql", FileCategory.Code },
        // Media
        { ".mp3", FileCategory.Media }, { ".wav", FileCategory.Media }, { ".m4a", FileCategory.Media },
        { ".mp4", FileCategory.Media }, { ".mov", FileCategory.Media }, { ".avi", FileCategory.Media },
        { ".mkv", FileCategory.Media }, { ".webm", FileCategory.Media },
        // Temporary
        { ".tmp", FileCategory.Temporary }, { ".log", FileCategory.Temporary }, { ".bak", FileCategory.Temporary }, { ".old", FileCategory.Temporary }
    };

    public string GetDesktopPath()
    {
        return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    }

    public string GetActualDesktopPath()
    {
        var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var oneDriveDesktop = Path.Combine(userProfile, "OneDrive", "Desktop");

        if (Directory.Exists(oneDriveDesktop))
        {
            return oneDriveDesktop;
        }

        return GetDesktopPath();
    }

    public async Task<List<DesktopFile>> ScanDesktopAsync()
    {
        var desktopPath = GetActualDesktopPath();
        return await Task.Run(() => ScanDirectory(desktopPath));
    }

    private List<DesktopFile> ScanDirectory(string path)
    {
        var files = new List<DesktopFile>();

        if (!Directory.Exists(path))
            return files;

        foreach (var filePath in Directory.GetFiles(path))
        {
            var fileInfo = new FileInfo(filePath);

            var desktopFile = new DesktopFile
            {
                Name = fileInfo.Name,
                Extension = fileInfo.Extension,
                FullPath = fileInfo.FullName,
                Size = fileInfo.Length,
                CreatedTime = fileInfo.CreationTime,
                ModifiedTime = fileInfo.LastWriteTime,
                IsHidden = (fileInfo.Attributes & FileAttributes.Hidden) != 0,
                IsSystem = (fileInfo.Attributes & FileAttributes.System) != 0,
                IsShortcut = fileInfo.Extension.Equals(".lnk", StringComparison.OrdinalIgnoreCase),
                IsDirectory = false,
                Category = CategorizeFile(fileInfo.Extension)
            };

            files.Add(desktopFile);
        }

        return files;
    }

    private FileCategory CategorizeFile(string extension)
    {
        if (string.IsNullOrEmpty(extension))
            return FileCategory.Others;

        return ExtensionMap.TryGetValue(extension, out var category) ? category : FileCategory.Others;
    }
}
