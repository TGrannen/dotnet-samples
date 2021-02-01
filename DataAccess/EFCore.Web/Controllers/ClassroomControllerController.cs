using EFCore.Web.Models;
using EFCore.Web.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCore.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClassroomControllerController : ControllerBase
    {
        private readonly SchoolContext _context;

        public ClassroomControllerController(SchoolContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("GetTopFive")]
        public async Task<List<Classroom>> Get()
        {
            var listAsync = await _context.Classrooms.Take(5).Include(x => x.Courses).ToListAsync();
            return listAsync;
        }

        [HttpGet]
        [Route("GetById")]
        public async Task<Classroom> GetById(int id)
        {
            return await _context.Classrooms.Include(x => x.Courses).SingleAsync(b => b.Id == id);
        }
    }
}