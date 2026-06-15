namespace DeskPilot.Core.Models;

public class OrganizeRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime ExecutedAt { get; set; } = DateTime.Now;
    public string Method { get; set; } = "Rule"; // Rule or AI
    public int TotalFiles { get; set; }
    public int MovedFiles { get; set; }
    public int SkippedFiles { get; set; }
    public OrganizePlan Plan { get; set; } = new();
    public UndoRecord? UndoRecord { get; set; }
}
