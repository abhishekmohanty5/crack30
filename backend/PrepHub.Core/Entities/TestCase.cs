namespace PrepHub.Core.Entities;

/// <summary>
/// A test case for a coding problem.
/// </summary>
public class TestCase
{
    public int Id { get; set; }
    public int ProblemId { get; set; }
    public string Input { get; set; } = string.Empty;
    public string ExpectedOutput { get; set; } = string.Empty;
    public bool IsHidden { get; set; }

    public CodingProblem Problem { get; set; } = null!;
}
