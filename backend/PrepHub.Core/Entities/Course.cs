namespace PrepHub.Core.Entities;

/// <summary>
/// A top-level course in the curriculum.
/// </summary>
public class Course
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Hours { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Topic> Topics { get; set; } = [];
}
