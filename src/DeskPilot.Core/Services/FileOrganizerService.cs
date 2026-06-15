using DeskPilot.Core.Interfaces;
using DeskPilot.Core.Models;

namespace DeskPilot.Core.Services;

public class FileOrganizerService : IFileOrganizerService
{
    public string ResolveTargetPath(string sourcePath, string desktopPath)
    {
        var sourceDir = Path.GetDirectoryName(sourcePath) ?? desktopPath;
        var sourceName = Path.GetFileName(sourcePath);
        var targetPath = Path.Combine(desktopPath, sourcePath);

        // If target already exists, find unique name
        if (File.Exists(targetPath) || Directory.Exists(targetPath))
        {
            var nameWithoutExt = Path.GetFileNameWithoutExtension(sourceName);
            var extension = Path.GetExtension(sourceName);
            var targetDir = Path.GetDirectoryName(targetPath) ?? desktopPath;

            for (int i = 1; i < 999; i++)
            {
                var newName = $"{nameWithoutExt} ({i}){extension}";
                var newPath = Path.Combine(targetDir, newName);
                if (!File.Exists(newPath) && !Directory.Exists(newPath))
                {
                    return newPath;
                }
            }
        }

        return targetPath;
    }

    public async Task<OrganizeRecord> ExecutePlanAsync(OrganizePlan plan, string desktopPath)
    {
        var record = new OrganizeRecord
        {
            Plan = plan,
            Method = "Rule",
            TotalFiles = plan.Moves.Count
        };

        var undoRecord = new UndoRecord();

        // Create folders first
        foreach (var folder in plan.FoldersToCreate)
        {
            var folderPath = Path.Combine(desktopPath, folder);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                undoRecord.CreatedFolders.Add(folder);
            }
        }

        // Move files
        foreach (var move in plan.Moves)
        {
            var sourcePath = Path.Combine(desktopPath, move.Source);
            var targetPath = Path.Combine(desktopPath, move.Target);

            if (!File.Exists(sourcePath))
            {
                record.SkippedFiles++;
                undoRecord.SkippedFiles.Add(new FileSkip { File = move.Source, Reason = "源文件不存在" });
                continue;
            }

            // Resolve duplicate names
            targetPath = ResolveTargetPath(move.Target, desktopPath);

            try
            {
                File.Move(sourcePath, targetPath);
                undoRecord.MovedFiles.Add(new UndoFileMove
                {
                    OriginalPath = sourcePath,
                    CurrentPath = targetPath
                });
                record.MovedFiles++;
            }
            catch (IOException)
            {
                record.SkippedFiles++;
                undoRecord.SkippedFiles.Add(new FileSkip { File = move.Source, Reason = "文件被占用" });
            }
        }

        record.UndoRecord = undoRecord;
        return await Task.FromResult(record);
    }

    public async Task<bool> UndoLastOrganizeAsync(OrganizeRecord record)
    {
        if (record.UndoRecord == null)
            return false;

        var undoRecord = record.UndoRecord;
        var allSuccess = true;

        // Move files back
        foreach (var fileMove in undoRecord.MovedFiles)
        {
            if (!File.Exists(fileMove.CurrentPath))
            {
                continue; // Skip - file doesn't exist
            }

            try
            {
                File.Move(fileMove.CurrentPath, fileMove.OriginalPath);
            }
            catch (IOException)
            {
                allSuccess = false; // Target path is occupied
            }
        }

        // Remove empty created folders (only if no user files exist in them)
        foreach (var folder in undoRecord.CreatedFolders)
        {
            var folderPath = Path.Combine(Path.GetDirectoryName(undoRecord.MovedFiles.FirstOrDefault()?.OriginalPath) ?? "", folder);
            if (Directory.Exists(folderPath))
            {
                try
                {
                    if (!Directory.EnumerateFileSystemEntries(folderPath).Any())
                    {
                        Directory.Delete(folderPath);
                    }
                }
                catch
                {
                    // Ignore - folder not empty or access denied
                }
            }
        }

        return await Task.FromResult(allSuccess);
    }
}
