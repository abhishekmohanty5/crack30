using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrepHub.Core.DTOs;
using PrepHub.Core.Interfaces;

namespace PrepHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class McqsController : ControllerBase
{
    private readonly IMcqService _mcqService;

    public McqsController(IMcqService mcqService)
    {
        _mcqService = mcqService;
    }

    [HttpGet("chapter/{chapterId:int}")]
    public async Task<ActionResult<PagedResponse<McqDto>>> GetChapterMcqs(
        int chapterId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _mcqService.GetChapterMcqsAsync(chapterId, page, pageSize);
        return Ok(result);
    }

    [HttpGet("chapter/{chapterId:int}/quiz")]
    public async Task<ActionResult<List<McqDto>>> GetQuiz(int chapterId, [FromQuery] int count = 10)
    {
        var questions = await _mcqService.GetQuizMcqsAsync(chapterId, count);
        return Ok(questions);
    }

    [HttpPost("{id:int}/attempt")]
    [Authorize]
    public async Task<ActionResult<McqAttemptResultDto>> SubmitAttempt(int id, [FromBody] McqAttemptRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        try
        {
            var result = await _mcqService.SubmitAttemptAsync(id, userId, request);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("{id:int}/attempts")]
    [Authorize]
    public async Task<ActionResult<List<McqAttemptDto>>> GetUserAttempts(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var attempts = await _mcqService.GetUserAttemptsAsync(id, userId);
        return Ok(attempts);
    }
}
