using PrepHub.Core.DTOs;

namespace PrepHub.Core.Interfaces;

/// <summary>
/// User content contribution service.
/// </summary>
public interface IContributionService
{
    Task<McqDto> SubmitMcqAsync(string userId, CreateMcqRequest request);
    Task<InterviewQuestionDto> SubmitInterviewQuestionAsync(string userId, CreateInterviewQuestionRequest request);
    Task<NoteDto> SubmitNoteAsync(string userId, CreateNoteRequest request);
}
