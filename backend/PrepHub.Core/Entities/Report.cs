using PrepHub.Core.Enums;

namespace PrepHub.Core.Entities;

/// <summary>
/// A user-submitted report about a content item.
/// </summary>
public class Report
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string ItemType { get; set; } = string.Empty;
    public int ItemId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public ReportStatus Status { get; set; } = ReportStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ApplicationUser User { get; set; } = null!;
}
