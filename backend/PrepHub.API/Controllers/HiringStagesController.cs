using Microsoft.AspNetCore.Mvc;
using PrepHub.Core.DTOs;
using PrepHub.Core.Interfaces;

namespace PrepHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HiringStagesController : ControllerBase
{
    private readonly IHiringStageService _service;

    public HiringStagesController(IHiringStageService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<HiringStageDto>>> GetAll()
    {
        var stages = await _service.GetAllStagesAsync();
        return Ok(stages);
    }
}
