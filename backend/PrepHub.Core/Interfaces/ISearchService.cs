using PrepHub.Core.DTOs;

namespace PrepHub.Core.Interfaces;

/// <summary>
/// Full-text search across content types.
/// </summary>
public interface ISearchService
{
    Task<List<SearchResultDto>> SearchAsync(string query, int? courseId, string? type, string? difficulty);
}
