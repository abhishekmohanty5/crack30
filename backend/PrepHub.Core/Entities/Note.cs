using PrepHub.Core.Enums;

namespace PrepHub.Core.Entities;

/// <summary>
/// Markdown revision note for a chapter.
/// </summary>
public class Note
{
    public int Id { get; set; }
    public int ChapterId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string MarkdownBody { get; set; } = string.Empty;
    public ContentStatus Status { get; set; } = ContentStatus.Pending;
    public string CreatedById { get; set; } = string.Empty;
    public string? ReviewedById { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Chapter Chapter { get; set; } = null!;
    public ApplicationUser CreatedBy { get; set; } = null!;
    public ApplicationUser? ReviewedBy { get; set; }
}
