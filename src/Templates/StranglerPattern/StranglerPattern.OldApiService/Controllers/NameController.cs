using Microsoft.AspNetCore.Mvc;

namespace StranglerPattern.OldApiService.Controllers;

[ApiController]
[Route("name")]
public class NameController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        // Simulate latency
        await Task.Delay(Random.Shared.Next(300, 500));

        return Ok(new { Text = "Old API -- ☹️" });
    }
}