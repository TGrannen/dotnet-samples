using Microsoft.AspNetCore.Mvc;

namespace FeatureFlags.Flagsmith.WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private readonly IFeatureService _service;
    private readonly IJsonFeatureService _jsonFeatureService;

    public TestController(IFeatureService service, IJsonFeatureService jsonFeatureService)
    {
        _service = service;
        _jsonFeatureService = jsonFeatureService;
    }

    [HttpGet]
    public async Task<IActionResult> Get(string split = "test_flagsmith")
    {
        var splitResults = await _service.IsEnabledAsync(split);
        return Ok(splitResults);
    }

    [HttpGet("json")]
    public async Task<IActionResult> GetConfig(string split = "json_flagsmith")
    {
        var splitResults = await _jsonFeatureService.GetConfiguration<Feature3Dto>(split);
        return Ok(splitResults);
    }

    public class Feature3Dto
    {
        public string Name { get; set; }
    }
}