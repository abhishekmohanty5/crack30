namespace PrepHub.Core.Entities;

/// <summary>
/// A topic within a course.
/// </summary>
public class Topic
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int SortOrder { get; set; }

    public Course Course { get; set; } = null!;
    public ICollection<Chapter> Chapters { get; set; } = [];
}
