using DeskPilot.Core.Models;
using DeskPilot.Core.Services;
using Xunit;

namespace DeskPilot.Tests.Services;

public class FileOrganizerServiceTests
{
    private readonly FileOrganizerService _service = new();

    [Fact]
    public void ResolveTargetPath_NoConflict_ReturnsOriginalPath()
    {
        var result = _service.ResolveTargetPath("Images", @"C:\Users\Test\Desktop");
        Assert.Equal(@"C:\Users\Test\Desktop\Images", result);
    }

    [Fact]
    public void ResolveTargetPath_FileExists_ReturnsIncrementedPath()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"DeskPilotTest_{Guid.NewGuid()}");
        Directory.CreateDirectory(tempDir);

        try
        {
            var targetPath = Path.Combine(tempDir, "photo.jpg");
            File.WriteAllText(targetPath, "original");

            var result = _service.ResolveTargetPath("photo.jpg", tempDir);

            Assert.NotEqual(targetPath, result);
            Assert.Contains("(1)", result);
        }
        finally
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public async Task ExecutePlanAsync_CreatesFolders()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"DeskPilotTest_{Guid.NewGuid()}");
        Directory.CreateDirectory(tempDir);

        try
        {
            // Create a test file
            var testFile = Path.Combine(tempDir, "test.txt");
            await File.WriteAllTextAsync(testFile, "test");

            var plan = new OrganizePlan
            {
                FoldersToCreate = new List<string> { "Documents" },
                Moves = new List<FileMove>
                {
                    new() { Source = "test.txt", Target = Path.Combine("Documents", "test.txt"), Reason = "test" }
                }
            };

            var result = await _service.ExecutePlanAsync(plan, tempDir);

            Assert.True(Directory.Exists(Path.Combine(tempDir, "Documents")));
            Assert.True(File.Exists(Path.Combine(tempDir, "Documents", "test.txt")));
            Assert.False(File.Exists(testFile)); // Original should be moved
        }
        finally
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public async Task UndoLastOrganizeAsync_RestoresFiles()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"DeskPilotTest_{Guid.NewGuid()}");
        Directory.CreateDirectory(tempDir);

        try
        {
            // Create a test file
            var testFile = Path.Combine(tempDir, "test.txt");
            await File.WriteAllTextAsync(testFile, "test");

            var plan = new OrganizePlan
            {
                FoldersToCreate = new List<string> { "Documents" },
                Moves = new List<FileMove>
                {
                    new() { Source = "test.txt", Target = Path.Combine("Documents", "test.txt"), Reason = "test" }
                }
            };

            var result = await _service.ExecutePlanAsync(plan, tempDir);
            var undoResult = await _service.UndoLastOrganizeAsync(result);

            Assert.True(undoResult);
            Assert.True(File.Exists(testFile)); // Should be restored
            Assert.False(File.Exists(Path.Combine(tempDir, "Documents", "test.txt")));
        }
        finally
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
    }
}
