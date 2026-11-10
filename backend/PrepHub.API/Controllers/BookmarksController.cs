using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrepHub.Core.DTOs;
using PrepHub.Core.Interfaces;

namespace PrepHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookmarksController : ControllerBase
{
    private readonly IBookmarkService _service;

    public BookmarksController(IBookmarkService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<BookmarkDto>>> GetBookmarks()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var bookmarks = await _service.GetUserBookmarksAsync(userId);
        return Ok(bookmarks);
    }

    [HttpPost]
    public async Task<ActionResult<BookmarkDto>> AddBookmark([FromBody] CreateBookmarkRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var bookmark = await _service.AddBookmarkAsync(userId, request);
        return Ok(bookmark);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteBookmark(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        await _service.DeleteBookmarkAsync(id, userId);
        return NoContent();
    }
}
