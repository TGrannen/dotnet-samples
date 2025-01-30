using Microsoft.AspNetCore.Mvc;

namespace StranglerPattern.OldApiService.Controllers;

[ApiController]
[Route("some-old-api")]
public class SomeOldApiController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new { Text = "--This is crazy️--", Time = DateTime.Now });
}