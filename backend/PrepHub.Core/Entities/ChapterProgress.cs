namespace PrepHub.Core.Entities;

/// <summary>
/// Tracks a user's revision progress on a chapter.
/// </summary>
public class ChapterProgress
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int ChapterId { get; set; }
    public bool IsRevised { get; set; }
    public int RevisionCount { get; set; }
    public DateTime? LastRevisedAt { get; set; }

    public ApplicationUser User { get; set; } = null!;
    public Chapter Chapter { get; set; } = null!;
}
