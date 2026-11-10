using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PrepHub.Core.Entities;

namespace PrepHub.Infrastructure.Data;

/// <summary>
/// Application database context with Identity and all domain entities.
/// </summary>
public class PrepHubDbContext(DbContextOptions<PrepHubDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Topic> Topics => Set<Topic>();
    public DbSet<Chapter> Chapters => Set<Chapter>();
    public DbSet<Note> Notes => Set<Note>();
    public DbSet<InterviewQuestion> InterviewQuestions => Set<InterviewQuestion>();
    public DbSet<McqQuestion> McqQuestions => Set<McqQuestion>();
    public DbSet<CodingProblem> CodingProblems => Set<CodingProblem>();
    public DbSet<TestCase> TestCases => Set<TestCase>();
    public DbSet<Submission> Submissions => Set<Submission>();
    public DbSet<McqAttempt> McqAttempts => Set<McqAttempt>();
    public DbSet<InterviewProgress> InterviewProgresses => Set<InterviewProgress>();
    public DbSet<ChapterProgress> ChapterProgresses => Set<ChapterProgress>();
    public DbSet<Bookmark> Bookmarks => Set<Bookmark>();
    public DbSet<MockTest> MockTests => Set<MockTest>();
    public DbSet<MockTestItem> MockTestItems => Set<MockTestItem>();
    public DbSet<HiringStage> HiringStages => Set<HiringStage>();
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ── Course ─────────────────────────────────────────────
        builder.Entity<Course>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.Title).HasMaxLength(200).IsRequired();
            e.Property(c => c.Description).HasMaxLength(2000);
            e.HasIndex(c => c.SortOrder);
        });

        // ── Topic ──────────────────────────────────────────────
        builder.Entity<Topic>(e =>
        {
            e.HasKey(t => t.Id);
            e.Property(t => t.Title).HasMaxLength(200).IsRequired();
            e.HasOne(t => t.Course).WithMany(c => c.Topics).HasForeignKey(t => t.CourseId).OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(t => new { t.CourseId, t.SortOrder });
        });

        // ── Chapter ────────────────────────────────────────────
        builder.Entity<Chapter>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.Title).HasMaxLength(200).IsRequired();
            e.HasOne(c => c.Topic).WithMany(t => t.Chapters).HasForeignKey(c => c.TopicId).OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(c => new { c.TopicId, c.SortOrder });
        });

        // ── Note ───────────────────────────────────────────────
        builder.Entity<Note>(e =>
        {
            e.HasKey(n => n.Id);
            e.Property(n => n.Title).HasMaxLength(300).IsRequired();
            e.HasOne(n => n.Chapter).WithMany(c => c.Notes).HasForeignKey(n => n.ChapterId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(n => n.CreatedBy).WithMany(u => u.Notes).HasForeignKey(n => n.CreatedById).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(n => n.ReviewedBy).WithMany().HasForeignKey(n => n.ReviewedById).OnDelete(DeleteBehavior.SetNull);
            e.HasIndex(n => new { n.ChapterId, n.Status });
        });

        // ── InterviewQuestion ──────────────────────────────────
        builder.Entity<InterviewQuestion>(e =>
        {
            e.HasKey(q => q.Id);
            e.Property(q => q.Question).HasMaxLength(2000).IsRequired();
            e.Property(q => q.ModelAnswer).HasMaxLength(5000);
            e.Property(q => q.Tags).HasMaxLength(500);
            e.HasOne(q => q.Chapter).WithMany(c => c.InterviewQuestions).HasForeignKey(q => q.ChapterId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(q => q.CreatedBy).WithMany().HasForeignKey(q => q.CreatedById).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(q => q.ReviewedBy).WithMany().HasForeignKey(q => q.ReviewedById).OnDelete(DeleteBehavior.SetNull);
            e.HasIndex(q => new { q.ChapterId, q.Status });
            e.HasIndex(q => q.Difficulty);
        });

        // ── McqQuestion ────────────────────────────────────────
        builder.Entity<McqQuestion>(e =>
        {
            e.HasKey(q => q.Id);
            e.Property(q => q.Question).HasMaxLength(2000).IsRequired();
            e.Property(q => q.OptionA).HasMaxLength(1000).IsRequired();
            e.Property(q => q.OptionB).HasMaxLength(1000).IsRequired();
            e.Property(q => q.OptionC).HasMaxLength(1000).IsRequired();
            e.Property(q => q.OptionD).HasMaxLength(1000).IsRequired();
            e.Property(q => q.Explanation).HasMaxLength(3000);
            e.Property(q => q.Tags).HasMaxLength(500);
            e.HasOne(q => q.Chapter).WithMany(c => c.McqQuestions).HasForeignKey(q => q.ChapterId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(q => q.CreatedBy).WithMany().HasForeignKey(q => q.CreatedById).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(q => q.ReviewedBy).WithMany().HasForeignKey(q => q.ReviewedById).OnDelete(DeleteBehavior.SetNull);
            e.HasIndex(q => new { q.ChapterId, q.Status });
            e.HasIndex(q => q.Difficulty);
        });

        // ── CodingProblem ──────────────────────────────────────
        builder.Entity<CodingProblem>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Title).HasMaxLength(300).IsRequired();
            e.Property(p => p.Statement).HasMaxLength(10000).IsRequired();
            e.Property(p => p.Constraints).HasMaxLength(2000);
            e.Property(p => p.Tags).HasMaxLength(500);
            e.Property(p => p.StarterCodeJson).HasMaxLength(10000);
            e.HasOne(p => p.Chapter).WithMany(c => c.CodingProblems).HasForeignKey(p => p.ChapterId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(p => p.CreatedBy).WithMany().HasForeignKey(p => p.CreatedById).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(p => new { p.ChapterId, p.Status });
        });

        // ── TestCase ───────────────────────────────────────────
        builder.Entity<TestCase>(e =>
        {
            e.HasKey(t => t.Id);
            e.HasOne(t => t.Problem).WithMany(p => p.TestCases).HasForeignKey(t => t.ProblemId).OnDelete(DeleteBehavior.Cascade);
        });

        // ── Submission ─────────────────────────────────────────
        builder.Entity<Submission>(e =>
        {
            e.HasKey(s => s.Id);
            e.Property(s => s.Language).HasMaxLength(50);
            e.Property(s => s.Verdict).HasMaxLength(50);
            e.HasOne(s => s.User).WithMany(u => u.Submissions).HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(s => s.Problem).WithMany(p => p.Submissions).HasForeignKey(s => s.ProblemId).OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(s => new { s.UserId, s.ProblemId });
        });

        // ── McqAttempt ─────────────────────────────────────────
        builder.Entity<McqAttempt>(e =>
        {
            e.HasKey(a => a.Id);
            e.HasOne(a => a.User).WithMany(u => u.McqAttempts).HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(a => a.Mcq).WithMany(q => q.Attempts).HasForeignKey(a => a.McqId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(a => a.Test).WithMany(t => t.McqAttempts).HasForeignKey(a => a.TestId).OnDelete(DeleteBehavior.SetNull);
            e.HasIndex(a => new { a.UserId, a.McqId });
        });

        // ── InterviewProgress ──────────────────────────────────
        builder.Entity<InterviewProgress>(e =>
        {
            e.HasKey(p => p.Id);
            e.HasOne(p => p.User).WithMany(u => u.InterviewProgresses).HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(p => p.Question).WithMany(q => q.Progresses).HasForeignKey(p => p.QuestionId).OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(p => new { p.UserId, p.QuestionId }).IsUnique();
        });

        // ── ChapterProgress ───────────────────────────────────
        builder.Entity<ChapterProgress>(e =>
        {
            e.HasKey(p => p.Id);
            e.HasOne(p => p.User).WithMany(u => u.ChapterProgresses).HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(p => p.Chapter).WithMany(c => c.ChapterProgresses).HasForeignKey(p => p.ChapterId).OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(p => new { p.UserId, p.ChapterId }).IsUnique();
        });

        // ── Bookmark ───────────────────────────────────────────
        builder.Entity<Bookmark>(e =>
        {
            e.HasKey(b => b.Id);
            e.Property(b => b.ItemType).HasMaxLength(50).IsRequired();
            e.HasOne(b => b.User).WithMany(u => u.Bookmarks).HasForeignKey(b => b.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(b => new { b.UserId, b.ItemType, b.ItemId }).IsUnique();
        });

        // ── MockTest ───────────────────────────────────────────
        builder.Entity<MockTest>(e =>
        {
            e.HasKey(t => t.Id);
            e.Property(t => t.Score).HasPrecision(5, 2);
            e.HasOne(t => t.User).WithMany(u => u.MockTests).HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // ── MockTestItem ───────────────────────────────────────
        builder.Entity<MockTestItem>(e =>
        {
            e.HasKey(i => i.Id);
            e.Property(i => i.ItemType).HasMaxLength(50);
            e.HasOne(i => i.Test).WithMany(t => t.Items).HasForeignKey(i => i.TestId).OnDelete(DeleteBehavior.Cascade);
        });

        // ── HiringStage ────────────────────────────────────────
        builder.Entity<HiringStage>(e =>
        {
            e.HasKey(h => h.Id);
            e.Property(h => h.BatchOrCompany).HasMaxLength(200).IsRequired();
            e.Property(h => h.Title).HasMaxLength(500).IsRequired();
            e.HasIndex(h => h.SortOrder);
        });

        // ── Report ─────────────────────────────────────────────
        builder.Entity<Report>(e =>
        {
            e.HasKey(r => r.Id);
            e.Property(r => r.ItemType).HasMaxLength(50).IsRequired();
            e.Property(r => r.Reason).HasMaxLength(2000).IsRequired();
            e.HasOne(r => r.User).WithMany(u => u.Reports).HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(r => r.Status);
        });

        // ── RefreshToken ───────────────────────────────────────
        builder.Entity<RefreshToken>(e =>
        {
            e.HasKey(t => t.Id);
            e.Property(t => t.Token).HasMaxLength(500).IsRequired();
            e.HasOne(t => t.User).WithMany(u => u.RefreshTokens).HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(t => t.Token).IsUnique();
        });
    }
}
