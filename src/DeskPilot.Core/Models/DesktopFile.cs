namespace DeskPilot.Core.Models;

public class DesktopFile
{
    public string Name { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public string FullPath { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime ModifiedTime { get; set; }
    public bool IsHidden { get; set; }
    public bool IsSystem { get; set; }
    public bool IsShortcut { get; set; }
    public bool IsDirectory { get; set; }

    public FileCategory Category { get; set; } = FileCategory.Others;

    public string RelativePath => Path.GetFileName(FullPath);
}
