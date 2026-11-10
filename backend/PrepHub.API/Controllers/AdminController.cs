using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrepHub.Core.DTOs;
using PrepHub.Core.Interfaces;

namespace PrepHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("pending")]
    public async Task<ActionResult<PagedResponse<PendingContributionDto>>> GetPending(
        [FromQuery] string? type = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _adminService.GetPendingContributionsAsync(type, page, pageSize);
        return Ok(result);
    }

    [HttpPost("{type}/{id:int}/approve")]
    public async Task<IActionResult> Approve(string type, int id, [FromBody] AdminActionRequest? request)
    {
        var adminUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(adminUserId)) return Unauthorized();

        await _adminService.ApproveAsync(type, id, adminUserId, request?.Comment);
        return NoContent();
    }

    [HttpPost("{type}/{id:int}/reject")]
    public async Task<IActionResult> Reject(string type, int id, [FromBody] AdminActionRequest? request)
    {
        var adminUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(adminUserId)) return Unauthorized();

        await _adminService.RejectAsync(type, id, adminUserId, request?.Comment);
        return NoContent();
    }

    [HttpPost("import")]
    public async Task<IActionResult> BulkImport([FromBody] BulkImportRequest request)
    {
        var adminUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(adminUserId)) return Unauthorized();

        try
        {
            await _adminService.BulkImportAsync(request, adminUserId);
            return Ok(new { message = "Bulk import completed successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Import failed: {ex.Message}" });
        }
    }

    [HttpGet("analytics")]
    public async Task<ActionResult<AnalyticsDto>> GetAnalytics()
    {
        var analytics = await _adminService.GetAnalyticsAsync();
        return Ok(analytics);
    }
}
