namespace PrepHub.Core.Entities;

/// <summary>
/// A chapter within a topic — the primary unit of learning content.
/// </summary>
public class Chapter
{
    public int Id { get; set; }
    public int TopicId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int SortOrder { get; set; }

    public Topic Topic { get; set; } = null!;
    public ICollection<Note> Notes { get; set; } = [];
    public ICollection<InterviewQuestion> InterviewQuestions { get; set; } = [];
    public ICollection<McqQuestion> McqQuestions { get; set; } = [];
    public ICollection<CodingProblem> CodingProblems { get; set; } = [];
    public ICollection<ChapterProgress> ChapterProgresses { get; set; } = [];
}
