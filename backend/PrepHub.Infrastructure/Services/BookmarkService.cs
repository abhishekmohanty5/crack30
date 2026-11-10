using Microsoft.EntityFrameworkCore;
using PrepHub.Core.DTOs;
using PrepHub.Core.Entities;
using PrepHub.Core.Interfaces;
using PrepHub.Infrastructure.Data;

namespace PrepHub.Infrastructure.Services;

public class BookmarkService : IBookmarkService
{
    private readonly PrepHubDbContext _context;

    public BookmarkService(PrepHubDbContext context)
    {
        _context = context;
    }

    public async Task<List<BookmarkDto>> GetUserBookmarksAsync(string userId)
    {
        return await _context.Bookmarks
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .Select(b => new BookmarkDto(b.Id, b.ItemType, b.ItemId, b.CreatedAt))
            .ToListAsync();
    }

    public async Task<BookmarkDto> AddBookmarkAsync(string userId, CreateBookmarkRequest request)
    {
        var existing = await _context.Bookmarks
            .FirstOrDefaultAsync(b => b.UserId == userId && b.ItemType == request.ItemType && b.ItemId == request.ItemId);

        if (existing != null)
            return new BookmarkDto(existing.Id, existing.ItemType, existing.ItemId, existing.CreatedAt);

        var bookmark = new Bookmark
        {
            UserId = userId,
            ItemType = request.ItemType,
            ItemId = request.ItemId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Bookmarks.Add(bookmark);
        await _context.SaveChangesAsync();

        return new BookmarkDto(bookmark.Id, bookmark.ItemType, bookmark.ItemId, bookmark.CreatedAt);
    }

    public async Task DeleteBookmarkAsync(int bookmarkId, string userId)
    {
        var bookmark = await _context.Bookmarks
            .FirstOrDefaultAsync(b => b.Id == bookmarkId && b.UserId == userId);

        if (bookmark != null)
        {
            _context.Bookmarks.Remove(bookmark);
            await _context.SaveChangesAsync();
        }
    }
}
