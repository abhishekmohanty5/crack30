namespace PrepHub.Core.DTOs;

// ======================== AUTH DTOs ========================

/// <summary>Request body for user registration.</summary>
public record RegisterRequest(string Email, string Password, string Name, string? Batch);

/// <summary>Request body for user login.</summary>
public record LoginRequest(string Email, string Password);

/// <summary>Request to refresh an access token.</summary>
public record RefreshRequest(string RefreshToken);

/// <summary>Request to revoke a refresh token on logout.</summary>
public record LogoutRequest(string RefreshToken);

/// <summary>Response returned after successful authentication.</summary>
public record AuthResponse(string AccessToken, string RefreshToken, UserDto User);

/// <summary>Basic user info returned in auth responses.</summary>
public record UserDto(string Id, string Name, string Email, string Role);

/// <summary>Detailed user profile.</summary>
public record UserProfileDto(
    string Id,
    string Name,
    string Email,
    string? Batch,
    string? TargetCompany,
    DateTime? InterviewDate,
    DateTime CreatedAt,
    string Role);

/// <summary>Request to update user profile.</summary>
public record UpdateProfileRequest(string? Name, string? Batch, string? TargetCompany, DateTime? InterviewDate);

// ======================== COURSE / TOPIC / CHAPTER DTOs ========================

/// <summary>Course summary for list view.</summary>
public record CourseListDto(
    int Id,
    string Title,
    string Description,
    int Hours,
    int SortOrder,
    int TopicCount,
    int ChapterCount,
    double ProgressPercent);

/// <summary>Course detail with nested topics.</summary>
public record CourseDetailDto(
    int Id,
    string Title,
    string Description,
    int Hours,
    int SortOrder,
    List<TopicDto> Topics);

/// <summary>Topic with chapters.</summary>
public record TopicDto(int Id, string Title, int SortOrder, List<ChapterSummaryDto> Chapters);

/// <summary>Chapter summary used in topic listings.</summary>
public record ChapterSummaryDto(int Id, string Title, int SortOrder, bool IsRevised, int RevisionCount);

/// <summary>Full chapter details with content counts.</summary>
public record ChapterDetailDto(
    int Id,
    string Title,
    int SortOrder,
    int NoteCount,
    int McqCount,
    int InterviewQuestionCount,
    int CodingProblemCount,
    bool IsRevised,
    int RevisionCount,
    DateTime? LastRevisedAt);

// ======================== NOTE DTOs ========================

/// <summary>Note summary.</summary>
public record NoteDto(int Id, string Title, string MarkdownBody, string CreatedByName, DateTime CreatedAt);

/// <summary>Request to contribute a note.</summary>
public record CreateNoteRequest(int ChapterId, string Title, string MarkdownBody);

// ======================== MCQ DTOs ========================

/// <summary>MCQ for practice (hides correct answer until attempted).</summary>
public record McqDto(
    int Id,
    string Question,
    string OptionA,
    string OptionB,
    string OptionC,
    string OptionD,
    string Difficulty,
    string Tags,
    int ChapterId);

/// <summary>MCQ with answer details (shown after attempt).</summary>
public record McqWithAnswerDto(
    int Id,
    string Question,
    string OptionA,
    string OptionB,
    string OptionC,
    string OptionD,
    char CorrectOption,
    string Explanation,
    string Difficulty,
    string Tags);

/// <summary>Request to submit an MCQ answer.</summary>
public record McqAttemptRequest(char ChosenOption, int TimeTakenSec);

/// <summary>Result of an MCQ attempt.</summary>
public record McqAttemptResultDto(bool IsCorrect, char CorrectOption, string Explanation);

/// <summary>Past MCQ attempt record.</summary>
public record McqAttemptDto(int Id, char ChosenOption, bool IsCorrect, int TimeTakenSec, DateTime CreatedAt);

/// <summary>Request to contribute an MCQ.</summary>
public record CreateMcqRequest(
    int ChapterId,
    string Question,
    string OptionA,
    string OptionB,
    string OptionC,
    string OptionD,
    char CorrectOption,
    string Explanation,
    string Difficulty,
    string Tags);

// ======================== INTERVIEW QUESTION DTOs ========================

/// <summary>Interview question for display.</summary>
public record InterviewQuestionDto(
    int Id,
    string Question,
    string ModelAnswer,
    string Difficulty,
    string Frequency,
    string Tags,
    string? UserStatus);

/// <summary>Request to update interview progress.</summary>
public record UpdateInterviewProgressRequest(string Status);

/// <summary>Interview progress record.</summary>
public record InterviewProgressDto(int QuestionId, string Status, DateTime UpdatedAt);

/// <summary>Request to contribute an interview question.</summary>
public record CreateInterviewQuestionRequest(
    int ChapterId,
    string Question,
    string ModelAnswer,
    string Difficulty,
    string Frequency,
    string Tags);

// ======================== PROGRESS DTOs ========================

/// <summary>Course-level progress.</summary>
public record CourseProgressDto(int CourseId, string CourseTitle, int TotalChapters, int RevisedChapters, double ProgressPercent);

/// <summary>Chapter-level progress detail.</summary>
public record ChapterProgressDto(int ChapterId, string ChapterTitle, string CourseName, bool IsRevised, int RevisionCount, DateTime? LastRevisedAt);

/// <summary>Weak area recommendation.</summary>
public record WeakAreaDto(int ChapterId, string ChapterTitle, string CourseName, int TotalAttempts, int CorrectAttempts, double AccuracyPercent);

// ======================== BOOKMARK DTOs ========================

/// <summary>Bookmark record.</summary>
public record BookmarkDto(int Id, string ItemType, int ItemId, DateTime CreatedAt);

/// <summary>Request to create a bookmark.</summary>
public record CreateBookmarkRequest(string ItemType, int ItemId);

// ======================== SEARCH DTOs ========================

/// <summary>Search result.</summary>
public record SearchResultDto(string Type, int Id, string Title, string Snippet, int ChapterId, string ChapterTitle);

// ======================== ADMIN DTOs ========================

/// <summary>Pending contribution for admin review.</summary>
public record PendingContributionDto(
    int Id,
    string Type,
    string Title,
    string CreatedByName,
    DateTime CreatedAt,
    string? Preview);

/// <summary>Admin action to approve/reject.</summary>
public record AdminActionRequest(string? Comment);

/// <summary>Platform analytics summary.</summary>
public record AnalyticsDto(
    int TotalUsers,
    int TotalCourses,
    int TotalMcqs,
    int TotalInterviewQuestions,
    int TotalNotes,
    int PendingContributions,
    int TotalAttempts);

/// <summary>Bulk import request.</summary>
public record BulkImportRequest(string Type, string JsonData);

// ======================== HIRING STAGE DTOs ========================

/// <summary>Hiring stage with countdown.</summary>
public record HiringStageDto(int Id, string BatchOrCompany, string Title, DateOnly StageDate, int SortOrder, int DaysRemaining);

// ======================== PAGINATION ========================

/// <summary>Paginated response wrapper.</summary>
public record PagedResponse<T>(List<T> Items, int TotalCount, int Page, int PageSize);
