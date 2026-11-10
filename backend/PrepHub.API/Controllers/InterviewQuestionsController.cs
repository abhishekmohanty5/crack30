using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrepHub.Core.DTOs;
using PrepHub.Core.Interfaces;

namespace PrepHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InterviewQuestionsController : ControllerBase
{
    private readonly IInterviewQuestionService _service;

    public InterviewQuestionsController(IInterviewQuestionService service)
    {
        _service = service;
    }

    [HttpGet("chapter/{chapterId:int}")]
    public async Task<ActionResult<PagedResponse<InterviewQuestionDto>>> GetChapterQuestions(
        int chapterId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _service.GetChapterQuestionsAsync(chapterId, userId, page, pageSize);
        return Ok(result);
    }

    [HttpPut("{id:int}/progress")]
    [Authorize]
    public async Task<ActionResult<InterviewProgressDto>> UpdateProgress(int id, [FromBody] UpdateInterviewProgressRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        try
        {
            var result = await _service.UpdateProgressAsync(id, userId, request);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("progress")]
    [Authorize]
    public async Task<ActionResult<List<InterviewProgressDto>>> GetUserProgress()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var list = await _service.GetUserProgressAsync(userId);
        return Ok(list);
    }
}
