using Microsoft.AspNetCore.Mvc;

namespace ReverseProxy.OcelotDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ValuesController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new string[] { "value1", "value2" });
    }
}