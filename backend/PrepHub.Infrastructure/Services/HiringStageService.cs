using Microsoft.EntityFrameworkCore;
using PrepHub.Core.DTOs;
using PrepHub.Core.Interfaces;
using PrepHub.Infrastructure.Data;

namespace PrepHub.Infrastructure.Services;

public class HiringStageService : IHiringStageService
{
    private readonly PrepHubDbContext _context;

    public HiringStageService(PrepHubDbContext context)
    {
        _context = context;
    }

    public async Task<List<HiringStageDto>> GetAllStagesAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var stages = await _context.HiringStages
            .OrderBy(s => s.SortOrder)
            .Select(s => new
            {
                s.Id,
                s.BatchOrCompany,
                s.Title,
                s.StageDate,
                s.SortOrder
            })
            .ToListAsync();

        return stages.Select(s =>
        {
            var daysRemaining = s.StageDate.DayNumber - today.DayNumber;
            return new HiringStageDto(
                s.Id,
                s.BatchOrCompany,
                s.Title,
                s.StageDate,
                s.SortOrder,
                daysRemaining > 0 ? daysRemaining : 0);
        }).ToList();
    }
}
