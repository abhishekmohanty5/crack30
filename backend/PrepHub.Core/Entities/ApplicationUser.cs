using Microsoft.AspNetCore.Identity;

namespace PrepHub.Core.Entities;

/// <summary>
/// Application user extending ASP.NET Identity with profile fields.
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>Full display name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Training batch identifier (e.g. "2026-B1").</summary>
    public string? Batch { get; set; }

    /// <summary>Company the student is targeting for placement.</summary>
    public string? TargetCompany { get; set; }

    /// <summary>Expected interview date.</summary>
    public DateTime? InterviewDate { get; set; }

    /// <summary>Account creation timestamp (UTC).</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
    public ICollection<Note> Notes { get; set; } = [];
    public ICollection<McqAttempt> McqAttempts { get; set; } = [];
    public ICollection<InterviewProgress> InterviewProgresses { get; set; } = [];
    public ICollection<ChapterProgress> ChapterProgresses { get; set; } = [];
    public ICollection<Bookmark> Bookmarks { get; set; } = [];
    public ICollection<MockTest> MockTests { get; set; } = [];
    public ICollection<Submission> Submissions { get; set; } = [];
    public ICollection<Report> Reports { get; set; } = [];
}
