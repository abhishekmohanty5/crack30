using PrepHub.Core.Enums;

namespace PrepHub.Core.Entities;

/// <summary>
/// An interview question associated with a chapter.
/// </summary>
public class InterviewQuestion
{
    public int Id { get; set; }
    public int ChapterId { get; set; }
    public string Question { get; set; } = string.Empty;
    public string ModelAnswer { get; set; } = string.Empty;
    public Difficulty Difficulty { get; set; }
    public Frequency Frequency { get; set; }
    public string Tags { get; set; } = string.Empty;
    public ContentStatus Status { get; set; } = ContentStatus.Pending;
    public string CreatedById { get; set; } = string.Empty;
    public string? ReviewedById { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Chapter Chapter { get; set; } = null!;
    public ApplicationUser CreatedBy { get; set; } = null!;
    public ApplicationUser? ReviewedBy { get; set; }
    public ICollection<InterviewProgress> Progresses { get; set; } = [];
}
