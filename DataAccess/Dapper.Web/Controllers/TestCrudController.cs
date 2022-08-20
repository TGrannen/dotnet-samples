namespace Dapper.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class TestCrudController : ControllerBase
{
    public TestCrudController()
    {
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(string? searchStr)
    {
        return Ok(searchStr);
    }
}