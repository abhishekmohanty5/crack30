using Microsoft.EntityFrameworkCore;
using PrepHub.Core.DTOs;
using PrepHub.Core.Entities;
using PrepHub.Core.Enums;
using PrepHub.Core.Interfaces;
using PrepHub.Infrastructure.Data;

namespace PrepHub.Infrastructure.Services;

public class ContributionService : IContributionService
{
    private readonly PrepHubDbContext _context;

    public ContributionService(PrepHubDbContext context)
    {
        _context = context;
    }

    public async Task<McqDto> SubmitMcqAsync(string userId, CreateMcqRequest request)
    {
        if (!Enum.TryParse<Difficulty>(request.Difficulty, true, out var difficulty))
            difficulty = Difficulty.Medium;

        var mcq = new McqQuestion
        {
            ChapterId = request.ChapterId,
            Question = request.Question,
            OptionA = request.OptionA,
            OptionB = request.OptionB,
            OptionC = request.OptionC,
            OptionD = request.OptionD,
            CorrectOption = char.ToUpperInvariant(request.CorrectOption),
            Explanation = request.Explanation,
            Difficulty = difficulty,
            Tags = request.Tags,
            Status = ContentStatus.Pending,
            CreatedById = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.McqQuestions.Add(mcq);
        await _context.SaveChangesAsync();

        return new McqDto(
            mcq.Id,
            mcq.Question,
            mcq.OptionA,
            mcq.OptionB,
            mcq.OptionC,
            mcq.OptionD,
            mcq.Difficulty.ToString(),
            mcq.Tags ?? string.Empty,
            mcq.ChapterId);
    }

    public async Task<InterviewQuestionDto> SubmitInterviewQuestionAsync(string userId, CreateInterviewQuestionRequest request)
    {
        if (!Enum.TryParse<Difficulty>(request.Difficulty, true, out var difficulty))
            difficulty = Difficulty.Medium;

        if (!Enum.TryParse<Frequency>(request.Frequency, true, out var frequency))
            frequency = Frequency.Common;

        var question = new InterviewQuestion
        {
            ChapterId = request.ChapterId,
            Question = request.Question,
            ModelAnswer = request.ModelAnswer,
            Difficulty = difficulty,
            Frequency = frequency,
            Tags = request.Tags,
            Status = ContentStatus.Pending,
            CreatedById = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.InterviewQuestions.Add(question);
        await _context.SaveChangesAsync();

        return new InterviewQuestionDto(
            question.Id,
            question.Question,
            question.ModelAnswer,
            question.Difficulty.ToString(),
            question.Frequency.ToString(),
            question.Tags ?? string.Empty,
            null);
    }

    public async Task<NoteDto> SubmitNoteAsync(string userId, CreateNoteRequest request)
    {
        var note = new Note
        {
            ChapterId = request.ChapterId,
            Title = request.Title,
            MarkdownBody = request.MarkdownBody,
            Status = ContentStatus.Pending,
            CreatedById = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Notes.Add(note);
        await _context.SaveChangesAsync();

        return new NoteDto(
            note.Id,
            note.Title,
            note.MarkdownBody,
            "Contributor",
            note.CreatedAt);
    }
}
