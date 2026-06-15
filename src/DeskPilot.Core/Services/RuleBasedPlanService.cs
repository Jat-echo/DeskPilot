using DeskPilot.Core.Interfaces;
using DeskPilot.Core.Models;

namespace DeskPilot.Core.Services;

public class RuleBasedPlanService : IOrganizePlanService
{
    private static readonly Dictionary<FileCategory, string> CategoryFolders = new()
    {
        { FileCategory.Image, "Images" },
        { FileCategory.Document, "Documents" },
        { FileCategory.Spreadsheet, "Spreadsheets" },
        { FileCategory.Presentation, "Presentations" },
        { FileCategory.Archive, "Archives" },
        { FileCategory.Installer, "Installers" },
        { FileCategory.Code, "Code" },
        { FileCategory.Media, "Media" },
        { FileCategory.Temporary, "Temporary" },
        { FileCategory.Others, "Others" }
    };

    public OrganizePlan GenerateRuleBasedPlan(List<DesktopFile> files)
    {
        var plan = new OrganizePlan
        {
            Warnings = new List<string> { "本次不会删除任何文件", "同名文件将自动重命名" }
        };

        foreach (var file in files)
        {
            // Skip shortcuts, hidden, system files
            if (file.IsShortcut)
            {
                plan.Skipped.Add(new FileSkip { File = file.Name, Reason = "快捷方式默认不处理" });
                continue;
            }

            if (file.IsHidden)
            {
                plan.Skipped.Add(new FileSkip { File = file.Name, Reason = "隐藏文件默认不处理" });
                continue;
            }

            if (file.IsSystem)
            {
                plan.Skipped.Add(new FileSkip { File = file.Name, Reason = "系统文件不处理" });
                continue;
            }

            // Generate target path
            var folderName = CategoryFolders[file.Category];
            var targetPath = $"{folderName}/{file.Name}";

            plan.Moves.Add(new FileMove
            {
                Source = file.Name,
                Target = targetPath,
                Reason = $"属于 {file.Category} 分类"
            });

            // Add folder if not already added
            if (!plan.FoldersToCreate.Contains(folderName))
            {
                plan.FoldersToCreate.Add(folderName);
            }
        }

        return plan;
    }
}
