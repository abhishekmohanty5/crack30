namespace PrepHub.Core.Entities;

/// <summary>
/// A timed mock test session.
/// </summary>
public class MockTest
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string ConfigJson { get; set; } = "{}";
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? SubmittedAt { get; set; }
    public decimal? Score { get; set; }

    public ApplicationUser User { get; set; } = null!;
    public ICollection<MockTestItem> Items { get; set; } = [];
    public ICollection<McqAttempt> McqAttempts { get; set; } = [];
}
