using DeskPilot.Core.Models;

namespace DeskPilot.Core.Interfaces;

public interface IOrganizePlanService
{
    OrganizePlan GenerateRuleBasedPlan(List<DesktopFile> files);
}
