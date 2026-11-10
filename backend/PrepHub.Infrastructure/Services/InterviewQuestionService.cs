using Microsoft.EntityFrameworkCore;
using PrepHub.Core.DTOs;
using PrepHub.Core.Entities;
using PrepHub.Core.Enums;
using PrepHub.Core.Interfaces;
using PrepHub.Infrastructure.Data;

namespace PrepHub.Infrastructure.Services;

public class InterviewQuestionService : IInterviewQuestionService
{
    private readonly PrepHubDbContext _context;

    public InterviewQuestionService(PrepHubDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResponse<InterviewQuestionDto>> GetChapterQuestionsAsync(int chapterId, string? userId, int page, int pageSize)
    {
        var query = _context.InterviewQuestions
            .Where(q => q.ChapterId == chapterId && q.Status == ContentStatus.Approved)
            .OrderBy(q => q.Id);

        var total = await query.CountAsync();
        var questions = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var userProgressMap = new Dictionary<int, string>();
        if (!string.IsNullOrEmpty(userId))
        {
            var qIds = questions.Select(q => q.Id).ToList();
            userProgressMap = await _context.InterviewProgresses
                .Where(p => p.UserId == userId && qIds.Contains(p.QuestionId))
                .ToDictionaryAsync(p => p.QuestionId, p => p.Status.ToString());
        }

        var items = questions.Select(q => new InterviewQuestionDto(
            q.Id,
            q.Question,
            q.ModelAnswer ?? string.Empty,
            q.Difficulty.ToString(),
            q.Frequency.ToString(),
            q.Tags ?? string.Empty,
            userProgressMap.TryGetValue(q.Id, out var status) ? status : null
        )).ToList();

        return new PagedResponse<InterviewQuestionDto>(items, total, page, pageSize);
    }

    public async Task<InterviewProgressDto> UpdateProgressAsync(int questionId, string userId, UpdateInterviewProgressRequest request)
    {
        var question = await _context.InterviewQuestions.FindAsync(questionId);
        if (question == null || question.Status != ContentStatus.Approved)
            throw new KeyNotFoundException($"Question with ID {questionId} not found.");

        if (!Enum.TryParse<InterviewConfidence>(request.Status, true, out var confidence))
            throw new ArgumentException("Status must be Confident or NeedRevision.");

        var progress = await _context.InterviewProgresses
            .FirstOrDefaultAsync(p => p.UserId == userId && p.QuestionId == questionId);

        if (progress == null)
        {
            progress = new InterviewProgress
            {
                UserId = userId,
                QuestionId = questionId,
                Status = confidence,
                UpdatedAt = DateTime.UtcNow
            };
            _context.InterviewProgresses.Add(progress);
        }
        else
        {
            progress.Status = confidence;
            progress.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return new InterviewProgressDto(questionId, confidence.ToString(), progress.UpdatedAt);
    }

    public async Task<List<InterviewProgressDto>> GetUserProgressAsync(string userId)
    {
        return await _context.InterviewProgresses
            .Where(p => p.UserId == userId)
            .Select(p => new InterviewProgressDto(p.QuestionId, p.Status.ToString(), p.UpdatedAt))
            .ToListAsync();
    }
}
