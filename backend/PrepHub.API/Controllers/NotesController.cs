using Microsoft.AspNetCore.Mvc;
using PrepHub.Core.DTOs;
using PrepHub.Core.Interfaces;

namespace PrepHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotesController : ControllerBase
{
    private readonly INoteService _noteService;

    public NotesController(INoteService noteService)
    {
        _noteService = noteService;
    }

    [HttpGet("chapter/{chapterId:int}")]
    public async Task<ActionResult<List<NoteDto>>> GetChapterNotes(int chapterId)
    {
        var notes = await _noteService.GetChapterNotesAsync(chapterId);
        return Ok(notes);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<NoteDto>> GetById(int id)
    {
        try
        {
            var note = await _noteService.GetNoteAsync(id);
            return Ok(note);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
