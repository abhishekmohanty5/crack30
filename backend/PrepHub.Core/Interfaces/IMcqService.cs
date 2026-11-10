using PrepHub.Core.DTOs;

namespace PrepHub.Core.Interfaces;

/// <summary>
/// MCQ question and attempt service.
/// </summary>
public interface IMcqService
{
    Task<PagedResponse<McqDto>> GetChapterMcqsAsync(int chapterId, int page, int pageSize);
    Task<List<McqDto>> GetQuizMcqsAsync(int chapterId, int count);
    Task<McqAttemptResultDto> SubmitAttemptAsync(int mcqId, string userId, McqAttemptRequest request);
    Task<List<McqAttemptDto>> GetUserAttemptsAsync(int mcqId, string userId);
}
