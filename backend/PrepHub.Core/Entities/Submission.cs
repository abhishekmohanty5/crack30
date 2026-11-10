namespace PrepHub.Core.Entities;

/// <summary>
/// A code submission for a coding problem.
/// </summary>
public class Submission
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int ProblemId { get; set; }
    public string Language { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Verdict { get; set; } = string.Empty;
    public int? RuntimeMs { get; set; }
    public int? MemoryKb { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ApplicationUser User { get; set; } = null!;
    public CodingProblem Problem { get; set; } = null!;
}
