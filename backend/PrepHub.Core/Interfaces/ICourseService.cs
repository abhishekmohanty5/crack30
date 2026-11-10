using PrepHub.Core.DTOs;

namespace PrepHub.Core.Interfaces;

/// <summary>
/// Course listing and detail service.
/// </summary>
public interface ICourseService
{
    Task<List<CourseListDto>> GetAllCoursesAsync(string? userId);
    Task<CourseDetailDto> GetCourseDetailAsync(int courseId, string? userId);
    Task<List<TopicDto>> GetCourseTopicsAsync(int courseId, string? userId);
}
