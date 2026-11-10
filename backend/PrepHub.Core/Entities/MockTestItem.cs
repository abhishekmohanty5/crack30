namespace PrepHub.Core.Entities;

/// <summary>
/// A single item within a mock test.
/// </summary>
public class MockTestItem
{
    public int Id { get; set; }
    public int TestId { get; set; }
    public string ItemType { get; set; } = string.Empty;
    public int ItemId { get; set; }
    public string? Answer { get; set; }
    public bool? IsCorrect { get; set; }

    public MockTest Test { get; set; } = null!;
}
