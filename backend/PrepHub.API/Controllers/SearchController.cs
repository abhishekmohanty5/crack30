using Microsoft.AspNetCore.Mvc;
using PrepHub.Core.DTOs;
using PrepHub.Core.Interfaces;

namespace PrepHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly ISearchService _searchService;

    public SearchController(ISearchService searchService)
    {
        _searchService = searchService;
    }

    [HttpGet]
    public async Task<ActionResult<List<SearchResultDto>>> Search(
        [FromQuery] string q,
        [FromQuery] int? course = null,
        [FromQuery] string? type = null,
        [FromQuery] string? difficulty = null)
    {
        var results = await _searchService.SearchAsync(q, course, type, difficulty);
        return Ok(results);
    }
}
