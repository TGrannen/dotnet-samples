using Microsoft.AspNetCore.Mvc;

namespace EFCore.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class CourseController : ControllerBase
{
    private readonly SchoolContext _context;

    public CourseController(SchoolContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("GetTopFive")]
    public async Task<List<Course>> Get()
    {
        var listAsync = await _context.Courses.OrderBy(x => x.Title).Take(5).ToListAsync();
        return listAsync;
    }

    [HttpGet]
    [Route("GetById")]
    public async Task<Course> GetById(int id)
    {
        return await _context.Courses.SingleAsync(b => b.CourseId == id);
    }
}