using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrepHub.Core.DTOs;
using PrepHub.Core.Interfaces;

namespace PrepHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ContributionsController : ControllerBase
{
    private readonly IContributionService _service;

    public ContributionsController(IContributionService service)
    {
        _service = service;
    }

    [HttpPost("mcq")]
    public async Task<ActionResult<McqDto>> SubmitMcq([FromBody] CreateMcqRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _service.SubmitMcqAsync(userId, request);
        return Ok(result);
    }

    [HttpPost("interview")]
    public async Task<ActionResult<InterviewQuestionDto>> SubmitInterview([FromBody] CreateInterviewQuestionRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _service.SubmitInterviewQuestionAsync(userId, request);
        return Ok(result);
    }

    [HttpPost("note")]
    public async Task<ActionResult<NoteDto>> SubmitNote([FromBody] CreateNoteRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _service.SubmitNoteAsync(userId, request);
        return Ok(result);
    }
}
