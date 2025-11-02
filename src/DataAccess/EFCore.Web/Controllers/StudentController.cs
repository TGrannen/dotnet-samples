using Microsoft.AspNetCore.Mvc;
using PhenX.EntityFrameworkCore.BulkInsert.Extensions;

namespace EFCore.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class StudentController(SchoolContext context, DataGenerator generator) : ControllerBase
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

    [HttpGet]
    [Route("GetTotalStudentCount")]
    public async Task<long> GetTotalStudentCount()
    {
        return await context.Students.CountAsync();
    }

    [HttpPost]
    [Route("BulkInsert")]
    public async Task<int> BulkInsert(int numberOfStudents = 10000, int batchSize = 1000)
    {
        var students = generator.GenerateStudents(numberOfStudents);
        foreach (var batch in students.Chunk(batchSize))
        {
            await context.ExecuteBulkInsertAsync(batch);
        }

        return numberOfStudents;
    }
}