namespace PrepHub.Core.Entities;

/// <summary>
/// A stage in the hiring/placement process.
/// </summary>
public class HiringStage
{
    public int Id { get; set; }
    public string BatchOrCompany { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateOnly StageDate { get; set; }
    public int SortOrder { get; set; }
}
