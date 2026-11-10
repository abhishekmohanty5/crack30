namespace PrepHub.Core.Entities;

/// <summary>
/// Records a user's answer to a single MCQ.
/// </summary>
public class McqAttempt
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int McqId { get; set; }
    public char ChosenOption { get; set; }
    public bool IsCorrect { get; set; }
    public int TimeTakenSec { get; set; }
    public int? TestId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ApplicationUser User { get; set; } = null!;
    public McqQuestion Mcq { get; set; } = null!;
    public MockTest? Test { get; set; }
}
