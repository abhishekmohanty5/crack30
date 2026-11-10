using PrepHub.Core.DTOs;

namespace PrepHub.Core.Interfaces;

/// <summary>
/// Interview question service.
/// </summary>
public interface IInterviewQuestionService
{
    Task<PagedResponse<InterviewQuestionDto>> GetChapterQuestionsAsync(int chapterId, string? userId, int page, int pageSize);
    Task<InterviewProgressDto> UpdateProgressAsync(int questionId, string userId, UpdateInterviewProgressRequest request);
    Task<List<InterviewProgressDto>> GetUserProgressAsync(string userId);
}
