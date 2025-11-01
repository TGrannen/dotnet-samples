using Microsoft.AspNetCore.Mvc;

namespace EFCore.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class EnrollmentsController : ControllerBase
{
    private readonly SchoolContext _context;

    public EnrollmentsController(SchoolContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("GetTopFive")]
    public async Task<List<Enrollment>> Get()
    {
        return await _context.Enrollments.OrderBy(x => x.EnrollmentId).Take(5).ToListAsync();
    }

    [HttpGet]
    [Route("GetById")]
    public async Task<List<Enrollment>> GetById(int studentId)
    {
        return await _context.Enrollments.Where(x => x.StudentId == studentId).ToListAsync();
    }
}