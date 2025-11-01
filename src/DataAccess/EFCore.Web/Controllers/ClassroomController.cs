using Microsoft.AspNetCore.Mvc;

namespace EFCore.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class ClassroomController : ControllerBase
{
    private readonly SchoolContext _context;

    public ClassroomController(SchoolContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("GetTopFive")]
    public async Task<List<Classroom>> Get()
    {
        var listAsync = await _context.Classrooms.OrderBy(x => x.RoomNumber).Take(5).Include(x => x.Courses).ToListAsync();
        return listAsync;
    }

    [HttpGet]
    [Route("GetById")]
    public async Task<Classroom> GetById(int id)
    {
        return await _context.Classrooms.Include(x => x.Courses).SingleAsync(b => b.Id == id);
    }
}