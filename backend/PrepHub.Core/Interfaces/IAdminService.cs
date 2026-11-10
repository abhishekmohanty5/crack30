using PrepHub.Core.DTOs;

namespace PrepHub.Core.Interfaces;

/// <summary>
/// Admin moderation and analytics service.
/// </summary>
public interface IAdminService
{
    Task<PagedResponse<PendingContributionDto>> GetPendingContributionsAsync(string? type, int page, int pageSize);
    Task ApproveAsync(string type, int id, string adminUserId, string? comment);
    Task RejectAsync(string type, int id, string adminUserId, string? comment);
    Task BulkImportAsync(BulkImportRequest request, string adminUserId);
    Task<AnalyticsDto> GetAnalyticsAsync();
}
