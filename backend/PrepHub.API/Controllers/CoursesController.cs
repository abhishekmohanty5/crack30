using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using PrepHub.Core.DTOs;
using PrepHub.Core.Interfaces;

namespace PrepHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpGet]
    public async Task<ActionResult<List<CourseListDto>>> GetAll()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var courses = await _courseService.GetAllCoursesAsync(userId);
        return Ok(courses);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CourseDetailDto>> GetById(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        try
        {
            var course = await _courseService.GetCourseDetailAsync(id, userId);
            return Ok(course);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("{id:int}/topics")]
    public async Task<ActionResult<List<TopicDto>>> GetTopics(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        try
        {
            var topics = await _courseService.GetCourseTopicsAsync(id, userId);
            return Ok(topics);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
