using DeskPilot.Core.Models;

namespace DeskPilot.Core.Interfaces;

public interface IAiPlanningService
{
    string GeneratePrompt(List<DesktopFile> files, string userRequest);
    AiPlanningResult ParseResponse(string jsonResponse);
    Task<AiPlanningResult> SendRequestAsync(List<DesktopFile> files, string userRequest, AiProviderSettings settings);
}
