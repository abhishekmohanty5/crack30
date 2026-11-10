using Microsoft.EntityFrameworkCore;
using PrepHub.Core.DTOs;
using PrepHub.Core.Enums;
using PrepHub.Core.Interfaces;
using PrepHub.Infrastructure.Data;

namespace PrepHub.Infrastructure.Services;

public class NoteService : INoteService
{
    private readonly PrepHubDbContext _context;

    public NoteService(PrepHubDbContext context)
    {
        _context = context;
    }

    public async Task<List<NoteDto>> GetChapterNotesAsync(int chapterId)
    {
        return await _context.Notes
            .Include(n => n.CreatedBy)
            .Where(n => n.ChapterId == chapterId && n.Status == ContentStatus.Approved)
            .OrderBy(n => n.CreatedAt)
            .Select(n => new NoteDto(
                n.Id,
                n.Title,
                n.MarkdownBody,
                n.CreatedBy != null ? n.CreatedBy.Name : "Instructor",
                n.CreatedAt))
            .ToListAsync();
    }

    public async Task<NoteDto> GetNoteAsync(int noteId)
    {
        var note = await _context.Notes
            .Include(n => n.CreatedBy)
            .FirstOrDefaultAsync(n => n.Id == noteId && n.Status == ContentStatus.Approved);

        if (note == null)
            throw new KeyNotFoundException($"Note with ID {noteId} not found.");

        return new NoteDto(
            note.Id,
            note.Title,
            note.MarkdownBody,
            note.CreatedBy != null ? note.CreatedBy.Name : "Instructor",
            note.CreatedAt);
    }
}
