using PrepHub.Core.DTOs;

namespace PrepHub.Core.Interfaces;

/// <summary>
/// Hiring stage service.
/// </summary>
public interface IHiringStageService
{
    Task<List<HiringStageDto>> GetAllStagesAsync();
}
