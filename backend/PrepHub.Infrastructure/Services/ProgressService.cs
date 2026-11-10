using Microsoft.EntityFrameworkCore;
using PrepHub.Core.DTOs;
using PrepHub.Core.Interfaces;
using PrepHub.Infrastructure.Data;

namespace PrepHub.Infrastructure.Services;

public class ProgressService : IProgressService
{
    private readonly PrepHubDbContext _context;

    public ProgressService(PrepHubDbContext context)
    {
        _context = context;
    }

    public async Task<List<CourseProgressDto>> GetCourseProgressAsync(string userId)
    {
        var courses = await _context.Courses
            .OrderBy(c => c.SortOrder)
            .Select(c => new
            {
                c.Id,
                c.Title,
                ChapterIds = c.Topics.SelectMany(t => t.Chapters).Select(ch => ch.Id).ToList()
            })
            .ToListAsync();

        var revisedChapterIds = (await _context.ChapterProgresses
            .Where(cp => cp.UserId == userId && cp.IsRevised)
            .Select(cp => cp.ChapterId)
            .ToListAsync())
            .ToHashSet();

        return courses.Select(c =>
        {
            var total = c.ChapterIds.Count;
            var revised = c.ChapterIds.Count(id => revisedChapterIds.Contains(id));
            var pct = total > 0 ? Math.Round((double)revised / total * 100, 1) : 0;
            return new CourseProgressDto(c.Id, c.Title, total, revised, pct);
        }).ToList();
    }

    public async Task<List<ChapterProgressDto>> GetChapterProgressAsync(string userId)
    {
        var progresses = await _context.ChapterProgresses
            .Include(cp => cp.Chapter)
                .ThenInclude(ch => ch.Topic)
                    .ThenInclude(t => t.Course)
            .Where(cp => cp.UserId == userId)
            .OrderByDescending(cp => cp.LastRevisedAt)
            .Select(cp => new ChapterProgressDto(
                cp.ChapterId,
                cp.Chapter.Title,
                cp.Chapter.Topic.Course.Title,
                cp.IsRevised,
                cp.RevisionCount,
                cp.LastRevisedAt))
            .ToListAsync();

        return progresses;
    }

    public async Task<List<WeakAreaDto>> GetWeakAreasAsync(string userId)
    {
        var attempts = await _context.McqAttempts
            .Include(a => a.Mcq)
                .ThenInclude(m => m.Chapter)
                    .ThenInclude(ch => ch.Topic)
                        .ThenInclude(t => t.Course)
            .Where(a => a.UserId == userId)
            .GroupBy(a => new { a.Mcq.ChapterId, ChapterTitle = a.Mcq.Chapter.Title, CourseName = a.Mcq.Chapter.Topic.Course.Title })
            .Select(g => new
            {
                g.Key.ChapterId,
                g.Key.ChapterTitle,
                g.Key.CourseName,
                TotalAttempts = g.Count(),
                CorrectAttempts = g.Count(a => a.IsCorrect)
            })
            .ToListAsync();

        return attempts
            .Select(a => new WeakAreaDto(
                a.ChapterId,
                a.ChapterTitle,
                a.CourseName,
                a.TotalAttempts,
                a.CorrectAttempts,
                a.TotalAttempts > 0 ? Math.Round((double)a.CorrectAttempts / a.TotalAttempts * 100, 1) : 0))
            .OrderBy(w => w.AccuracyPercent)
            .Take(5)
            .ToList();
    }
}
