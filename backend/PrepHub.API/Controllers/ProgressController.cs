using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrepHub.Core.DTOs;
using PrepHub.Core.Interfaces;

namespace PrepHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProgressController : ControllerBase
{
    private readonly IProgressService _progressService;

    public ProgressController(IProgressService progressService)
    {
        _progressService = progressService;
    }

    [HttpGet("courses")]
    public async Task<ActionResult<List<CourseProgressDto>>> GetCourseProgress()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _progressService.GetCourseProgressAsync(userId);
        return Ok(result);
    }

    [HttpGet("chapters")]
    public async Task<ActionResult<List<ChapterProgressDto>>> GetChapterProgress()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _progressService.GetChapterProgressAsync(userId);
        return Ok(result);
    }

    [HttpGet("weak-areas")]
    public async Task<ActionResult<List<WeakAreaDto>>> GetWeakAreas()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _progressService.GetWeakAreasAsync(userId);
        return Ok(result);
    }
}
