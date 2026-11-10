using PrepHub.Core.Enums;

namespace PrepHub.Core.Entities;

/// <summary>
/// A coding problem linked to a chapter.
/// </summary>
public class CodingProblem
{
    public int Id { get; set; }
    public int ChapterId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Statement { get; set; } = string.Empty;
    public string Constraints { get; set; } = string.Empty;
    public Difficulty Difficulty { get; set; }
    public string Tags { get; set; } = string.Empty;
    public string StarterCodeJson { get; set; } = "{}";
    public ContentStatus Status { get; set; } = ContentStatus.Pending;
    public string CreatedById { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Chapter Chapter { get; set; } = null!;
    public ApplicationUser CreatedBy { get; set; } = null!;
    public ICollection<TestCase> TestCases { get; set; } = [];
    public ICollection<Submission> Submissions { get; set; } = [];
}
