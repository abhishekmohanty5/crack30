using PrepHub.Core.DTOs;

namespace PrepHub.Core.Interfaces;

/// <summary>
/// Chapter detail and revision tracking service.
/// </summary>
public interface IChapterService
{
    Task<ChapterDetailDto> GetChapterDetailAsync(int chapterId, string? userId);
    Task<ChapterDetailDto> MarkChapterRevisedAsync(int chapterId, string userId);
}
