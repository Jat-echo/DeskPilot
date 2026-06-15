using DeskPilot.Core.Models;
using DeskPilot.Core.Services;
using Xunit;

namespace DeskPilot.Tests.Services;

public class AiPlanningServiceTests
{
    private readonly AiPlanningService _service = new();

    [Fact]
    public void GeneratePrompt_ContainsFileList()
    {
        var files = new List<DesktopFile>
        {
            new() { Name = "photo.jpg", Extension = ".jpg", Size = 1024 * 1024, CreatedTime = DateTime.Now.AddDays(-1) },
            new() { Name = "doc.pdf", Extension = ".pdf", Size = 2048, CreatedTime = DateTime.Now }
        };

        var prompt = _service.GeneratePrompt(files, "整理图片和文档");

        Assert.Contains("photo.jpg", prompt);
        Assert.Contains("doc.pdf", prompt);
        Assert.Contains("整理图片和文档", prompt);
    }

    [Fact]
    public void GeneratePrompt_DoesNotContainAbsolutePaths()
    {
        var files = new List<DesktopFile>
        {
            new() { Name = "photo.jpg", Extension = ".jpg", FullPath = @"C:\Users\ZhangSan\Desktop\photo.jpg" }
        };

        var prompt = _service.GeneratePrompt(files, "");

        Assert.DoesNotContain("ZhangSan", prompt);
        Assert.DoesNotContain("C:\\", prompt);
    }

    [Fact]
    public void ParseResponse_ValidJson_ReturnsSuccess()
    {
        var json = @"{
            ""foldersToCreate"": [""Images""],
            ""moves"": [{""source"": ""photo.jpg"", ""target"": ""Images/photo.jpg"", ""reason"": ""图片""}],
            ""skipped"": [],
            ""warnings"": []
        }";

        var result = _service.ParseResponse(json);

        Assert.True(result.Success);
        Assert.NotNull(result.Plan);
        Assert.Single(result.Plan.FoldersToCreate);
        Assert.Single(result.Plan.Moves);
    }

    [Fact]
    public void ParseResponse_InvalidJson_ReturnsError()
    {
        var json = "这不是有效的JSON";

        var result = _service.ParseResponse(json);

        Assert.False(result.Success);
        Assert.Contains("JSON解析失败", result.ErrorMessage);
    }

    [Fact]
    public void ParseResponse_JsonWithExtraText_ExtractsJson()
    {
        var json = @"好的，这是整理计划：
{
    ""foldersToCreate"": [""Images""],
    ""moves"": [],
    ""skipped"": [],
    ""warnings"": []
}
请按照这个计划执行。";

        var result = _service.ParseResponse(json);

        Assert.True(result.Success);
        Assert.NotNull(result.Plan);
    }
}
