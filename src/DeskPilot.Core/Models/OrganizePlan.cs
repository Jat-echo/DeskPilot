namespace DeskPilot.Core.Models;

public class OrganizePlan
{
    public List<string> FoldersToCreate { get; set; } = new();
    public List<FileMove> Moves { get; set; } = new();
    public List<FileSkip> Skipped { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}

public class FileMove
{
    public string Source { get; set; } = string.Empty;
    public string Target { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}

public class FileSkip
{
    public string File { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}
