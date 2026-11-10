using PrepHub.Core.Enums;

namespace PrepHub.Core.Entities;

/// <summary>
/// Tracks a user's confidence on a specific interview question.
/// </summary>
public class InterviewProgress
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int QuestionId { get; set; }
    public InterviewConfidence Status { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ApplicationUser User { get; set; } = null!;
    public InterviewQuestion Question { get; set; } = null!;
}
