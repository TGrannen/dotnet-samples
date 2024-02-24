using Microsoft.AspNetCore.Mvc;

namespace Polly.Web.Controllers;

[ApiController]
public class TestController : ControllerBase
{
    private readonly ITestService _testService;

    public TestController(ITestService testService)
    {
        _testService = testService;
    }

    [HttpGet("Test")]
    public async Task<IActionResult> RunTest()
    {
        var value = await _testService.RunTest();
        return Ok(value);
    }
}