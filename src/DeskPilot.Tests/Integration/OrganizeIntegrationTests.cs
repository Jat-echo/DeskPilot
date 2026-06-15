using DeskPilot.Core.Models;
using DeskPilot.Core.Services;
using Xunit;

namespace DeskPilot.Tests.Integration;

public class OrganizeIntegrationTests
{
    [Fact]
    public void FullWorkflow_Scan_Plan_Execute()
    {
        var scannerService = new FileScannerService();
        var planService = new RuleBasedPlanService();
        var organizerService = new FileOrganizerService();

        var files = scannerService.ScanDesktopAsync().GetAwaiter().GetResult();
        var plan = planService.GenerateRuleBasedPlan(files);

        Assert.NotNull(plan);
        Assert.NotNull(plan.FoldersToCreate);
        Assert.NotNull(plan.Moves);
        Assert.NotNull(plan.Skipped);
        Assert.NotNull(plan.Warnings);
    }

    [Fact]
    public void Plan_RespectsSkipRules()
    {
        var files = new List<DesktopFile>
        {
            new() { Name = "test.lnk", IsShortcut = true, Category = FileCategory.Others },
            new() { Name = ".hidden", IsHidden = true, Category = FileCategory.Others },
            new() { Name = "photo.jpg", IsShortcut = false, IsHidden = false, IsSystem = false, Category = FileCategory.Image }
        };

        var planService = new RuleBasedPlanService();
        var plan = planService.GenerateRuleBasedPlan(files);

        Assert.Single(plan.Moves);
        Assert.Equal(2, plan.Skipped.Count);
    }

    [Fact]
    public void AiPlanningService_ParseResponse_HandlesValidJson()
    {
        var service = new AiPlanningService();
        var json = @"{
            ""foldersToCreate"": [""Images""],
            ""moves"": [{""source"": ""photo.jpg"", ""target"": ""Images/photo.jpg"", ""reason"": ""图片""}],
            ""skipped"": [],
            ""warnings"": []
        }";

        var result = service.ParseResponse(json);

        Assert.True(result.Success);
        Assert.NotNull(result.Plan);
    }

    [Fact]
    public void AiPlanningService_ParseResponse_HandlesInvalidJson()
    {
        var service = new AiPlanningService();
        var result = service.ParseResponse("invalid json");

        Assert.False(result.Success);
        Assert.Contains("JSON解析失败", result.ErrorMessage ?? "");
    }
}
