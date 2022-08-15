using Microsoft.AspNetCore.Mvc;
using Nest;
using Serilog;

namespace FullTextSearch.ElasticSearch.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly ElasticClient _client;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, ElasticClient client)
    {
        _logger = logger;
        _client = client;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
    }

    [HttpGet("test", Name = "test")]
    public async Task<IActionResult> Get(string? searchStr)
    {
        var searchResponse = await _client.SearchAsync<LogDocument>(s => s
            .Size(10)
            .Query(q => q.QueryString(qs => qs
                .Query(searchStr)
                .AllowLeadingWildcard(true)))
        );
        return Ok(searchResponse.Documents);
    }
}