using Microsoft.EntityFrameworkCore;
using PrepHub.Core.DTOs;
using PrepHub.Core.Enums;
using PrepHub.Core.Interfaces;
using PrepHub.Infrastructure.Data;

namespace PrepHub.Infrastructure.Services;

public class SearchService : ISearchService
{
    private readonly PrepHubDbContext _context;

    public SearchService(PrepHubDbContext context)
    {
        _context = context;
    }

    public async Task<List<SearchResultDto>> SearchAsync(string query, int? courseId, string? type, string? difficulty)
    {
        if (string.IsNullOrWhiteSpace(query))
            return new List<SearchResultDto>();

        var q = query.ToLower();
        var results = new List<SearchResultDto>();

        // MCQs
        if (string.IsNullOrEmpty(type) || type.Equals("mcq", StringComparison.OrdinalIgnoreCase))
        {
            var mcqQuery = _context.McqQuestions
                .Include(m => m.Chapter)
                    .ThenInclude(ch => ch.Topic)
                        .ThenInclude(t => t.Course)
                .Where(m => m.Status == ContentStatus.Approved &&
                            (m.Question.ToLower().Contains(q) || (m.Tags != null && m.Tags.ToLower().Contains(q))));

            if (courseId.HasValue)
                mcqQuery = mcqQuery.Where(m => m.Chapter.Topic.CourseId == courseId.Value);

            if (!string.IsNullOrEmpty(difficulty) && Enum.TryParse<Difficulty>(difficulty, true, out var diff))
                mcqQuery = mcqQuery.Where(m => m.Difficulty == diff);

            var mcqs = await mcqQuery.Take(20).ToListAsync();
            results.AddRange(mcqs.Select(m => new SearchResultDto(
                "MCQ",
                m.Id,
                m.Question.Length > 80 ? m.Question[..80] + "..." : m.Question,
                m.Question,
                m.ChapterId,
                m.Chapter.Title)));
        }

        // Interview questions
        if (string.IsNullOrEmpty(type) || type.Equals("interview", StringComparison.OrdinalIgnoreCase))
        {
            var intQuery = _context.InterviewQuestions
                .Include(i => i.Chapter)
                    .ThenInclude(ch => ch.Topic)
                        .ThenInclude(t => t.Course)
                .Where(i => i.Status == ContentStatus.Approved &&
                            (i.Question.ToLower().Contains(q) || (i.Tags != null && i.Tags.ToLower().Contains(q))));

            if (courseId.HasValue)
                intQuery = intQuery.Where(i => i.Chapter.Topic.CourseId == courseId.Value);

            if (!string.IsNullOrEmpty(difficulty) && Enum.TryParse<Difficulty>(difficulty, true, out var diff))
                intQuery = intQuery.Where(i => i.Difficulty == diff);

            var questions = await intQuery.Take(20).ToListAsync();
            results.AddRange(questions.Select(i => new SearchResultDto(
                "Interview",
                i.Id,
                i.Question.Length > 80 ? i.Question[..80] + "..." : i.Question,
                i.ModelAnswer ?? string.Empty,
                i.ChapterId,
                i.Chapter.Title)));
        }

        // Notes
        if (string.IsNullOrEmpty(type) || type.Equals("note", StringComparison.OrdinalIgnoreCase))
        {
            var noteQuery = _context.Notes
                .Include(n => n.Chapter)
                    .ThenInclude(ch => ch.Topic)
                        .ThenInclude(t => t.Course)
                .Where(n => n.Status == ContentStatus.Approved &&
                            (n.Title.ToLower().Contains(q) || n.MarkdownBody.ToLower().Contains(q)));

            if (courseId.HasValue)
                noteQuery = noteQuery.Where(n => n.Chapter.Topic.CourseId == courseId.Value);

            var notes = await noteQuery.Take(20).ToListAsync();
            results.AddRange(notes.Select(n => new SearchResultDto(
                "Note",
                n.Id,
                n.Title,
                n.MarkdownBody.Length > 120 ? n.MarkdownBody[..120] + "..." : n.MarkdownBody,
                n.ChapterId,
                n.Chapter.Title)));
        }

        return results;
    }
}
