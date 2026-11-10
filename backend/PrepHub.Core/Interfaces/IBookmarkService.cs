using PrepHub.Core.DTOs;

namespace PrepHub.Core.Interfaces;

/// <summary>
/// User bookmark service.
/// </summary>
public interface IBookmarkService
{
    Task<List<BookmarkDto>> GetUserBookmarksAsync(string userId);
    Task<BookmarkDto> AddBookmarkAsync(string userId, CreateBookmarkRequest request);
    Task DeleteBookmarkAsync(int bookmarkId, string userId);
}
