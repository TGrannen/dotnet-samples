using FullTextSearch.ElasticSearch.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Nest;

namespace FullTextSearch.ElasticSearch.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class ElasticController : ControllerBase
{
    private readonly ElasticClient _client;
    private readonly SeedService _seedService;

    public ElasticController(ElasticClient client, SeedService seedService)
    {
        _client = client;
        _seedService = seedService;
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(string? searchStr)
    {
        var searchResponse = await _client.SearchAsync<LogDocument>(s => s
            .Size(10)
            .Query(q => q.QueryString(qs => qs
                .Query(searchStr)
                .AllowLeadingWildcard(true)))
        );
        return Ok(searchResponse.Documents);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Get(string body)
    {
        var response = await _client.CreateDocumentAsync(new LogDocument()
        {
            Body = body.Trim()
        });
        return Ok(response.Id);
    }

    [HttpPost("random")]
    public async Task<IActionResult> Random(int count, CancellationToken token)
    {
        await _seedService.SeedData(count, token);
        return Ok();
    }
}