using Microsoft.EntityFrameworkCore;
using PrepHub.Core.DTOs;
using PrepHub.Core.Entities;
using PrepHub.Core.Enums;
using PrepHub.Core.Interfaces;
using PrepHub.Infrastructure.Data;

namespace PrepHub.Infrastructure.Services;

public class McqService : IMcqService
{
    private readonly PrepHubDbContext _context;

    public McqService(PrepHubDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResponse<McqDto>> GetChapterMcqsAsync(int chapterId, int page, int pageSize)
    {
        var query = _context.McqQuestions
            .Where(q => q.ChapterId == chapterId && q.Status == ContentStatus.Approved)
            .OrderBy(q => q.Id);

        var total = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(q => new McqDto(
                q.Id,
                q.Question,
                q.OptionA,
                q.OptionB,
                q.OptionC,
                q.OptionD,
                q.Difficulty.ToString(),
                q.Tags ?? string.Empty,
                q.ChapterId))
            .ToListAsync();

        return new PagedResponse<McqDto>(items, total, page, pageSize);
    }

    public async Task<List<McqDto>> GetQuizMcqsAsync(int chapterId, int count)
    {
        var questions = await _context.McqQuestions
            .Where(q => q.ChapterId == chapterId && q.Status == ContentStatus.Approved)
            .OrderBy(r => EF.Functions.Random())
            .Take(count)
            .Select(q => new McqDto(
                q.Id,
                q.Question,
                q.OptionA,
                q.OptionB,
                q.OptionC,
                q.OptionD,
                q.Difficulty.ToString(),
                q.Tags ?? string.Empty,
                q.ChapterId))
            .ToListAsync();

        return questions;
    }

    public async Task<McqAttemptResultDto> SubmitAttemptAsync(int mcqId, string userId, McqAttemptRequest request)
    {
        var mcq = await _context.McqQuestions.FindAsync(mcqId);
        if (mcq == null || mcq.Status != ContentStatus.Approved)
            throw new KeyNotFoundException($"MCQ with ID {mcqId} not found.");

        var normalizedChosen = char.ToUpperInvariant(request.ChosenOption);
        var normalizedCorrect = char.ToUpperInvariant(mcq.CorrectOption);
        var isCorrect = normalizedChosen == normalizedCorrect;

        var attempt = new McqAttempt
        {
            UserId = userId,
            McqId = mcqId,
            ChosenOption = normalizedChosen,
            IsCorrect = isCorrect,
            TimeTakenSec = request.TimeTakenSec,
            CreatedAt = DateTime.UtcNow
        };

        _context.McqAttempts.Add(attempt);
        await _context.SaveChangesAsync();

        return new McqAttemptResultDto(isCorrect, normalizedCorrect, mcq.Explanation ?? string.Empty);
    }

    public async Task<List<McqAttemptDto>> GetUserAttemptsAsync(int mcqId, string userId)
    {
        return await _context.McqAttempts
            .Where(a => a.McqId == mcqId && a.UserId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new McqAttemptDto(a.Id, a.ChosenOption, a.IsCorrect, a.TimeTakenSec, a.CreatedAt))
            .ToListAsync();
    }
}
