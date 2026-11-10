using PrepHub.Core.DTOs;

namespace PrepHub.Core.Interfaces;

/// <summary>
/// User progress aggregation service.
/// </summary>
public interface IProgressService
{
    Task<List<CourseProgressDto>> GetCourseProgressAsync(string userId);
    Task<List<ChapterProgressDto>> GetChapterProgressAsync(string userId);
    Task<List<WeakAreaDto>> GetWeakAreasAsync(string userId);
}
