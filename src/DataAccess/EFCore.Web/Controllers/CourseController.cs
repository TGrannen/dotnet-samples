using Microsoft.AspNetCore.Mvc;

namespace EFCore.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class CourseController(SchoolContext context) : ControllerBase
{
    [HttpGet]
    [Route("GetTopFive")]
    public async Task<List<Course>> Get()
    {
        var listAsync = await context.Courses.OrderBy(x => x.Title).Take(5).ToListAsync();
        return listAsync;
    }

    [HttpGet]
    [Route("GetById")]
    public async Task<Course> GetById(int id)
    {
        return await context.Courses.SingleAsync(b => b.CourseId == id);
    }
}