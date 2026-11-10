using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrepHub.Core.DTOs;
using PrepHub.Core.Interfaces;

namespace PrepHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChaptersController : ControllerBase
{
    private readonly IChapterService _chapterService;

    public ChaptersController(IChapterService chapterService)
    {
        _chapterService = chapterService;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ChapterDetailDto>> GetById(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        try
        {
            var chapter = await _chapterService.GetChapterDetailAsync(id, userId);
            return Ok(chapter);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPut("{id:int}/revise")]
    [Authorize]
    public async Task<ActionResult<ChapterDetailDto>> MarkRevised(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        try
        {
            var updated = await _chapterService.MarkChapterRevisedAsync(id, userId);
            return Ok(updated);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
