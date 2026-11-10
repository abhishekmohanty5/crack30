using Microsoft.EntityFrameworkCore;
using PrepHub.Core.DTOs;
using PrepHub.Core.Entities;
using PrepHub.Core.Enums;
using PrepHub.Core.Interfaces;
using PrepHub.Infrastructure.Data;

namespace PrepHub.Infrastructure.Services;

public class ChapterService : IChapterService
{
    private readonly PrepHubDbContext _context;

    public ChapterService(PrepHubDbContext context)
    {
        _context = context;
    }

    public async Task<ChapterDetailDto> GetChapterDetailAsync(int chapterId, string? userId)
    {
        var chapter = await _context.Chapters
            .Include(c => c.Topic)
                .ThenInclude(t => t.Course)
            .FirstOrDefaultAsync(c => c.Id == chapterId);

        if (chapter == null)
            throw new KeyNotFoundException($"Chapter with ID {chapterId} not found.");

        var noteCount = await _context.Notes
            .CountAsync(n => n.ChapterId == chapterId && n.Status == ContentStatus.Approved);

        var mcqCount = await _context.McqQuestions
            .CountAsync(q => q.ChapterId == chapterId && q.Status == ContentStatus.Approved);

        var interviewCount = await _context.InterviewQuestions
            .CountAsync(q => q.ChapterId == chapterId && q.Status == ContentStatus.Approved);

        var codingCount = await _context.CodingProblems
            .CountAsync(p => p.ChapterId == chapterId && p.Status == ContentStatus.Approved);

        ChapterProgress? progress = null;
        if (!string.IsNullOrEmpty(userId))
        {
            progress = await _context.ChapterProgresses
                .FirstOrDefaultAsync(cp => cp.UserId == userId && cp.ChapterId == chapterId);
        }

        return new ChapterDetailDto(
            chapter.Id,
            chapter.Title,
            chapter.SortOrder,
            noteCount,
            mcqCount,
            interviewCount,
            codingCount,
            progress?.IsRevised ?? false,
            progress?.RevisionCount ?? 0,
            progress?.LastRevisedAt);
    }

    public async Task<ChapterDetailDto> MarkChapterRevisedAsync(int chapterId, string userId)
    {
        var chapter = await _context.Chapters.FindAsync(chapterId);
        if (chapter == null)
            throw new KeyNotFoundException($"Chapter with ID {chapterId} not found.");

        var progress = await _context.ChapterProgresses
            .FirstOrDefaultAsync(cp => cp.UserId == userId && cp.ChapterId == chapterId);

        if (progress == null)
        {
            progress = new ChapterProgress
            {
                UserId = userId,
                ChapterId = chapterId,
                IsRevised = true,
                RevisionCount = 1,
                LastRevisedAt = DateTime.UtcNow
            };
            _context.ChapterProgresses.Add(progress);
        }
        else
        {
            progress.IsRevised = true;
            progress.RevisionCount += 1;
            progress.LastRevisedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return await GetChapterDetailAsync(chapterId, userId);
    }
}
