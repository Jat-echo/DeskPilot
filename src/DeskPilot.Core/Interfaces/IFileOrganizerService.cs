using DeskPilot.Core.Models;

namespace DeskPilot.Core.Interfaces;

public interface IFileOrganizerService
{
    Task<OrganizeRecord> ExecutePlanAsync(OrganizePlan plan, string desktopPath);
    Task<bool> UndoLastOrganizeAsync(OrganizeRecord record);
    string ResolveTargetPath(string sourcePath, string desktopPath);
}
