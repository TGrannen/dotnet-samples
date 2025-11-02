using Microsoft.AspNetCore.Mvc;

namespace EFCore.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class EnrollmentsController(SchoolContext context) : ControllerBase
{
    [HttpGet]
    [Route("GetTopFive")]
    public async Task<List<Enrollment>> Get()
    {
        return await context.Enrollments.OrderBy(x => x.EnrollmentId).Take(5).ToListAsync();
    }

    [HttpGet]
    [Route("GetById")]
    public async Task<List<Enrollment>> GetById(int studentId)
    {
        return await context.Enrollments.Where(x => x.StudentId == studentId).ToListAsync();
    }
}