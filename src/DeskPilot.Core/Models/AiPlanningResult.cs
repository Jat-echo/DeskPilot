namespace DeskPilot.Core.Models;

public class AiPlanningResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public OrganizePlan? Plan { get; set; }
}
