using Microsoft.AspNetCore.Mvc;

namespace EFCore.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class StudentController(SchoolContext context) : ControllerBase
{
    [HttpGet]
    [Route("GetTopFive")]
    public async Task<List<Student>> Get()
    {
        return await context.Students
            .Include(x => x.Enrollments)
            .ThenInclude(x => x.Course)
            .OrderBy(x => x.LastName)
            .Take(5)
            .ToListAsync();
    }

    [HttpGet]
    [Route("GetById")]
    public async Task<Student> GetById(int id)
    {
        return await context.Students.SingleAsync(b => b.Id == id);
    }
}