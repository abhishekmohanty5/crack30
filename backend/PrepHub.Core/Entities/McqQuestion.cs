using PrepHub.Core.Enums;

namespace PrepHub.Core.Entities;

/// <summary>
/// Multiple-choice question for quizzes and mock tests.
/// </summary>
public class McqQuestion
{
    public int Id { get; set; }
    public int ChapterId { get; set; }
    public string Question { get; set; } = string.Empty;
    public string OptionA { get; set; } = string.Empty;
    public string OptionB { get; set; } = string.Empty;
    public string OptionC { get; set; } = string.Empty;
    public string OptionD { get; set; } = string.Empty;
    public char CorrectOption { get; set; }
    public string Explanation { get; set; } = string.Empty;
    public Difficulty Difficulty { get; set; }
    public string Tags { get; set; } = string.Empty;
    public ContentStatus Status { get; set; } = ContentStatus.Pending;
    public string CreatedById { get; set; } = string.Empty;
    public string? ReviewedById { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Chapter Chapter { get; set; } = null!;
    public ApplicationUser CreatedBy { get; set; } = null!;
    public ApplicationUser? ReviewedBy { get; set; }
    public ICollection<McqAttempt> Attempts { get; set; } = [];
}
