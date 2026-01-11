using Microsoft.AspNetCore.Mvc;

namespace ReverseProxy.OcelotDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ValuesController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        string[] values = ["value1", "value2"];
        return Ok(values);
    }
}