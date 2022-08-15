using Microsoft.AspNetCore.Mvc;
using Nest;

namespace FullTextSearch.ElasticSearch.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class ElasticController : ControllerBase
{
    private readonly ElasticClient _client;

    public ElasticController(ElasticClient client)
    {
        _client = client;
    }

    [HttpGet("search", Name = "test")]
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