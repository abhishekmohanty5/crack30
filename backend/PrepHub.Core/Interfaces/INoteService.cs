using PrepHub.Core.DTOs;

namespace PrepHub.Core.Interfaces;

/// <summary>
/// Note retrieval service.
/// </summary>
public interface INoteService
{
    Task<List<NoteDto>> GetChapterNotesAsync(int chapterId);
    Task<NoteDto> GetNoteAsync(int noteId);
}
