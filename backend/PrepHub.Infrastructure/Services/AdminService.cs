using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PrepHub.Core.DTOs;
using PrepHub.Core.Entities;
using PrepHub.Core.Enums;
using PrepHub.Core.Interfaces;
using PrepHub.Infrastructure.Data;

namespace PrepHub.Infrastructure.Services;

public class AdminService : IAdminService
{
    private readonly PrepHubDbContext _context;

    public AdminService(PrepHubDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResponse<PendingContributionDto>> GetPendingContributionsAsync(string? type, int page, int pageSize)
    {
        var pendingList = new List<PendingContributionDto>();

        // MCQs
        if (string.IsNullOrEmpty(type) || type.Equals("mcq", StringComparison.OrdinalIgnoreCase))
        {
            var rawMcqs = await _context.McqQuestions
                .Include(m => m.CreatedBy)
                .Where(m => m.Status == ContentStatus.Pending)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();

            pendingList.AddRange(rawMcqs.Select(m => new PendingContributionDto(
                m.Id,
                "MCQ",
                m.Question.Length > 80 ? m.Question.Substring(0, 80) + "..." : m.Question,
                m.CreatedBy != null ? m.CreatedBy.Name : "Contributor",
                m.CreatedAt,
                $"A: {m.OptionA} | B: {m.OptionB} | Correct: {m.CorrectOption}")));
        }

        // Interview questions
        if (string.IsNullOrEmpty(type) || type.Equals("interview", StringComparison.OrdinalIgnoreCase))
        {
            var rawInts = await _context.InterviewQuestions
                .Include(i => i.CreatedBy)
                .Where(i => i.Status == ContentStatus.Pending)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            pendingList.AddRange(rawInts.Select(i => new PendingContributionDto(
                i.Id,
                "Interview",
                i.Question.Length > 80 ? i.Question.Substring(0, 80) + "..." : i.Question,
                i.CreatedBy != null ? i.CreatedBy.Name : "Contributor",
                i.CreatedAt,
                !string.IsNullOrEmpty(i.ModelAnswer) && i.ModelAnswer.Length > 120 ? i.ModelAnswer.Substring(0, 120) + "..." : i.ModelAnswer)));
        }

        // Notes
        if (string.IsNullOrEmpty(type) || type.Equals("note", StringComparison.OrdinalIgnoreCase))
        {
            var rawNotes = await _context.Notes
                .Include(n => n.CreatedBy)
                .Where(n => n.Status == ContentStatus.Pending)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            pendingList.AddRange(rawNotes.Select(n => new PendingContributionDto(
                n.Id,
                "Note",
                n.Title,
                n.CreatedBy != null ? n.CreatedBy.Name : "Contributor",
                n.CreatedAt,
                !string.IsNullOrEmpty(n.MarkdownBody) && n.MarkdownBody.Length > 120 ? n.MarkdownBody.Substring(0, 120) + "..." : n.MarkdownBody)));
        }

        var total = pendingList.Count;
        var paged = pendingList
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResponse<PendingContributionDto>(paged, total, page, pageSize);
    }

    public async Task ApproveAsync(string type, int id, string adminUserId, string? comment)
    {
        switch (type.ToLower())
        {
            case "mcq":
                var mcq = await _context.McqQuestions.FindAsync(id);
                if (mcq != null)
                {
                    mcq.Status = ContentStatus.Approved;
                    mcq.ReviewedById = adminUserId;
                }
                break;
            case "interview":
                var intQ = await _context.InterviewQuestions.FindAsync(id);
                if (intQ != null)
                {
                    intQ.Status = ContentStatus.Approved;
                    intQ.ReviewedById = adminUserId;
                }
                break;
            case "note":
                var note = await _context.Notes.FindAsync(id);
                if (note != null)
                {
                    note.Status = ContentStatus.Approved;
                    note.ReviewedById = adminUserId;
                }
                break;
        }

        await _context.SaveChangesAsync();
    }

    public async Task RejectAsync(string type, int id, string adminUserId, string? comment)
    {
        switch (type.ToLower())
        {
            case "mcq":
                var mcq = await _context.McqQuestions.FindAsync(id);
                if (mcq != null)
                {
                    mcq.Status = ContentStatus.Rejected;
                    mcq.ReviewedById = adminUserId;
                }
                break;
            case "interview":
                var intQ = await _context.InterviewQuestions.FindAsync(id);
                if (intQ != null)
                {
                    intQ.Status = ContentStatus.Rejected;
                    intQ.ReviewedById = adminUserId;
                }
                break;
            case "note":
                var note = await _context.Notes.FindAsync(id);
                if (note != null)
                {
                    note.Status = ContentStatus.Rejected;
                    note.ReviewedById = adminUserId;
                }
                break;
        }

        await _context.SaveChangesAsync();
    }

    public async Task BulkImportAsync(BulkImportRequest request, string adminUserId)
    {
        using var doc = JsonDocument.Parse(request.JsonData);
        var root = doc.RootElement;

        if (root.TryGetProperty("mcqs", out var mcqsEl))
        {
            foreach (var item in mcqsEl.EnumerateArray())
            {
                var chId = item.GetProperty("chapterId").GetInt32();
                var q = item.GetProperty("question").GetString() ?? "";
                var oa = item.GetProperty("optionA").GetString() ?? "";
                var ob = item.GetProperty("optionB").GetString() ?? "";
                var oc = item.GetProperty("optionC").GetString() ?? "";
                var od = item.GetProperty("optionD").GetString() ?? "";
                var co = item.GetProperty("correctOption").GetString()?.FirstOrDefault() ?? 'A';
                var exp = item.TryGetProperty("explanation", out var expEl) ? expEl.GetString() : "";
                var diffStr = item.TryGetProperty("difficulty", out var dEl) ? dEl.GetString() : "Medium";
                Enum.TryParse<Difficulty>(diffStr, true, out var diff);

                _context.McqQuestions.Add(new McqQuestion
                {
                    ChapterId = chId,
                    Question = q,
                    OptionA = oa,
                    OptionB = ob,
                    OptionC = oc,
                    OptionD = od,
                    CorrectOption = char.ToUpperInvariant(co),
                    Explanation = exp,
                    Difficulty = diff,
                    Status = ContentStatus.Approved,
                    CreatedById = adminUserId,
                    ReviewedById = adminUserId,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        if (root.TryGetProperty("interviewQuestions", out var iqsEl))
        {
            foreach (var item in iqsEl.EnumerateArray())
            {
                var chId = item.GetProperty("chapterId").GetInt32();
                var q = item.GetProperty("question").GetString() ?? "";
                var ans = item.GetProperty("modelAnswer").GetString() ?? "";
                var diffStr = item.TryGetProperty("difficulty", out var dEl) ? dEl.GetString() : "Medium";
                var freqStr = item.TryGetProperty("frequency", out var fEl) ? fEl.GetString() : "Common";
                Enum.TryParse<Difficulty>(diffStr, true, out var diff);
                Enum.TryParse<Frequency>(freqStr, true, out var freq);

                _context.InterviewQuestions.Add(new InterviewQuestion
                {
                    ChapterId = chId,
                    Question = q,
                    ModelAnswer = ans,
                    Difficulty = diff,
                    Frequency = freq,
                    Status = ContentStatus.Approved,
                    CreatedById = adminUserId,
                    ReviewedById = adminUserId,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        if (root.TryGetProperty("notes", out var notesEl))
        {
            foreach (var item in notesEl.EnumerateArray())
            {
                var chId = item.GetProperty("chapterId").GetInt32();
                var title = item.GetProperty("title").GetString() ?? "";
                var body = item.GetProperty("markdownBody").GetString() ?? "";

                _context.Notes.Add(new Note
                {
                    ChapterId = chId,
                    Title = title,
                    MarkdownBody = body,
                    Status = ContentStatus.Approved,
                    CreatedById = adminUserId,
                    ReviewedById = adminUserId,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task<AnalyticsDto> GetAnalyticsAsync()
    {
        var users = await _context.Users.CountAsync();
        var courses = await _context.Courses.CountAsync();
        var mcqs = await _context.McqQuestions.CountAsync(q => q.Status == ContentStatus.Approved);
        var interviewQs = await _context.InterviewQuestions.CountAsync(q => q.Status == ContentStatus.Approved);
        var notes = await _context.Notes.CountAsync(n => n.Status == ContentStatus.Approved);
        var pendingMcq = await _context.McqQuestions.CountAsync(q => q.Status == ContentStatus.Pending);
        var pendingInt = await _context.InterviewQuestions.CountAsync(q => q.Status == ContentStatus.Pending);
        var pendingNote = await _context.Notes.CountAsync(n => n.Status == ContentStatus.Pending);
        var totalAttempts = await _context.McqAttempts.CountAsync();

        return new AnalyticsDto(
            users,
            courses,
            mcqs,
            interviewQs,
            notes,
            pendingMcq + pendingInt + pendingNote,
            totalAttempts);
    }
}
