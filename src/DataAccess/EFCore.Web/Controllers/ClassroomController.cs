using Microsoft.AspNetCore.Mvc;

namespace EFCore.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class ClassroomController(SchoolContext context) : ControllerBase
{
    [HttpGet]
    [Route("GetTopFive")]
    public async Task<List<Classroom>> Get()
    {
        var listAsync = await context.Classrooms.OrderBy(x => x.RoomNumber).Take(5).Include(x => x.Courses).ToListAsync();
        return listAsync;
    }

    [HttpGet]
    [Route("GetById")]
    public async Task<Classroom> GetById(int id)
    {
        return await context.Classrooms.Include(x => x.Courses).SingleAsync(b => b.Id == id);
    }
}