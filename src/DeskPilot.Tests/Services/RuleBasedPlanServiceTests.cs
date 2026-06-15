using DeskPilot.Core.Models;
using DeskPilot.Core.Services;
using Xunit;

namespace DeskPilot.Tests.Services;

public class RuleBasedPlanServiceTests
{
    private readonly RuleBasedPlanService _service = new();

    [Fact]
    public void GenerateRuleBasedPlan_SortsImagesCorrectly()
    {
        var files = new List<DesktopFile>
        {
            new() { Name = "photo.jpg", Extension = ".jpg", Category = FileCategory.Image, IsShortcut = false, IsHidden = false, IsSystem = false }
        };

        var plan = _service.GenerateRuleBasedPlan(files);

        Assert.Single(plan.FoldersToCreate);
        Assert.Equal("Images", plan.FoldersToCreate[0]);
        Assert.Single(plan.Moves);
        Assert.Equal("photo.jpg", plan.Moves[0].Source);
        Assert.Contains("Images", plan.Moves[0].Target);
    }

    [Fact]
    public void GenerateRuleBasedPlan_SkipsShortcuts()
    {
        var files = new List<DesktopFile>
        {
            new() { Name = "Chrome.lnk", Extension = ".lnk", Category = FileCategory.Others, IsShortcut = true }
        };

        var plan = _service.GenerateRuleBasedPlan(files);

        Assert.Empty(plan.Moves);
        Assert.Single(plan.Skipped);
        Assert.Contains("快捷方式", plan.Skipped[0].Reason);
    }

    [Fact]
    public void GenerateRuleBasedPlan_SkipsHiddenFiles()
    {
        var files = new List<DesktopFile>
        {
            new() { Name = ".hidden", Extension = "", Category = FileCategory.Others, IsHidden = true, IsShortcut = false }
        };

        var plan = _service.GenerateRuleBasedPlan(files);

        Assert.Empty(plan.Moves);
        Assert.Single(plan.Skipped);
        Assert.Contains("隐藏文件", plan.Skipped[0].Reason);
    }

    [Fact]
    public void GenerateRuleBasedPlan_UnrecognizedGoesToOthers()
    {
        var files = new List<DesktopFile>
        {
            new() { Name = "unknown.xyz", Extension = ".xyz", Category = FileCategory.Others, IsShortcut = false, IsHidden = false, IsSystem = false }
        };

        var plan = _service.GenerateRuleBasedPlan(files);

        Assert.Contains("Others", plan.FoldersToCreate);
        Assert.Single(plan.Moves);
    }

    [Fact]
    public void GenerateRuleBasedPlan_HasWarnings()
    {
        var files = new List<DesktopFile>();

        var plan = _service.GenerateRuleBasedPlan(files);

        Assert.NotEmpty(plan.Warnings);
        Assert.Contains(plan.Warnings, w => w.Contains("不会删除"));
        Assert.Contains(plan.Warnings, w => w.Contains("同名文件"));
    }
}
