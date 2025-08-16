using FullTextSearch.ElasticSearch.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace FullTextSearch.ElasticSearch.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class ElasticController(ElasticClient client, SeedService seedService) : ControllerBase
{
    [HttpGet("search")]
    public async Task<IActionResult> Search(string? searchStr)
    {
        var searchResponse = await client.SearchAsync<LogDocument>(s => s
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
        var response = await client.CreateDocumentAsync(new LogDocument()
        {
            Body = body.Trim()
        });
        return Ok(response.Id);
    }

    [HttpPost("random")]
    public async Task<IActionResult> Random(int count, CancellationToken token)
    {
        await seedService.SeedData(count, token);
        return Ok();
    }
}