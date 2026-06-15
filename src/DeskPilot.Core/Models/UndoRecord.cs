namespace DeskPilot.Core.Models;

public class UndoRecord
{
    public List<UndoFileMove> MovedFiles { get; set; } = new();
    public List<string> CreatedFolders { get; set; } = new();
    public List<FileSkip> SkippedFiles { get; set; } = new();
}

public class UndoFileMove
{
    public string OriginalPath { get; set; } = string.Empty;
    public string CurrentPath { get; set; } = string.Empty;
}
