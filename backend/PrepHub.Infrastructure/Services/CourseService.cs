using Microsoft.EntityFrameworkCore;
using PrepHub.Core.DTOs;
using PrepHub.Core.Entities;
using PrepHub.Core.Interfaces;
using PrepHub.Infrastructure.Data;

namespace PrepHub.Infrastructure.Services;

public class CourseService : ICourseService
{
    private readonly PrepHubDbContext _context;

    public CourseService(PrepHubDbContext context)
    {
        _context = context;
    }

    public async Task<List<CourseListDto>> GetAllCoursesAsync(string? userId)
    {
        var courses = await _context.Courses
            .OrderBy(c => c.SortOrder)
            .Select(c => new
            {
                c.Id,
                c.Title,
                c.Description,
                c.Hours,
                c.SortOrder,
                TopicCount = c.Topics.Count,
                ChapterCount = c.Topics.SelectMany(t => t.Chapters).Count(),
                ChapterIds = c.Topics.SelectMany(t => t.Chapters).Select(ch => ch.Id).ToList()
            })
            .ToListAsync();

        var revisedChapterIds = new HashSet<int>();
        if (!string.IsNullOrEmpty(userId))
        {
            revisedChapterIds = (await _context.ChapterProgresses
                .Where(cp => cp.UserId == userId && cp.IsRevised)
                .Select(cp => cp.ChapterId)
                .ToListAsync())
                .ToHashSet();
        }

        return courses.Select(c =>
        {
            var revisedCount = c.ChapterIds.Count(id => revisedChapterIds.Contains(id));
            var progress = c.ChapterCount > 0 ? Math.Round((double)revisedCount / c.ChapterCount * 100, 1) : 0;

            return new CourseListDto(
                c.Id,
                c.Title,
                c.Description,
                c.Hours,
                c.SortOrder,
                c.TopicCount,
                c.ChapterCount,
                progress);
        }).ToList();
    }

    public async Task<CourseDetailDto> GetCourseDetailAsync(int courseId, string? userId)
    {
        var course = await _context.Courses
            .Include(c => c.Topics.OrderBy(t => t.SortOrder))
                .ThenInclude(t => t.Chapters.OrderBy(ch => ch.SortOrder))
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course == null)
            throw new KeyNotFoundException($"Course with ID {courseId} not found.");

        var userProgressMap = new Dictionary<int, ChapterProgress>();
        if (!string.IsNullOrEmpty(userId))
        {
            var chapterIds = course.Topics.SelectMany(t => t.Chapters).Select(ch => ch.Id).ToList();
            userProgressMap = await _context.ChapterProgresses
                .Where(cp => cp.UserId == userId && chapterIds.Contains(cp.ChapterId))
                .ToDictionaryAsync(cp => cp.ChapterId);
        }

        var topicsDto = course.Topics.Select(t => new TopicDto(
            t.Id,
            t.Title,
            t.SortOrder,
            t.Chapters.Select(ch =>
            {
                var hasProg = userProgressMap.TryGetValue(ch.Id, out var prog);
                return new ChapterSummaryDto(
                    ch.Id,
                    ch.Title,
                    ch.SortOrder,
                    hasProg && prog!.IsRevised,
                    hasProg ? prog!.RevisionCount : 0);
            }).ToList()
        )).ToList();

        return new CourseDetailDto(
            course.Id,
            course.Title,
            course.Description,
            course.Hours,
            course.SortOrder,
            topicsDto);
    }

    public async Task<List<TopicDto>> GetCourseTopicsAsync(int courseId, string? userId)
    {
        var detail = await GetCourseDetailAsync(courseId, userId);
        return detail.Topics;
    }
}
