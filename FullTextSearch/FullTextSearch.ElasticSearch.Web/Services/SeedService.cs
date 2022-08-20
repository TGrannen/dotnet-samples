using System.Diagnostics;
using Bogus;
using Nest;

namespace FullTextSearch.ElasticSearch.Web.Services;

public class SeedService
{
    private readonly ElasticClient _client;
    private readonly ILogger<SeedService> _logger;
    private readonly Faker<LogDocument> _logsFaker;

    public SeedService(ElasticClient client, ILogger<SeedService> logger)
    {
        Randomizer.Seed = new Random(123456);
        _client = client;
        _logger = logger;
        _logsFaker = new Faker<LogDocument>()
            .RuleFor(o => o.Id, Guid.NewGuid)
            .RuleFor(o => o.Body, f => f.Random.Words());
    }

    public async Task SeedData(int count)
    {
        var timer = new Stopwatch();
        timer.Start();

        _logger.LogInformation("Writing Documents");
        var documents = _logsFaker.Generate(count);
        foreach (var document in documents)
        {
            await _client.CreateDocumentAsync(new LogDocument()
            {
                Body = document.Body.Trim()
            });
        }

        timer.Stop();
        _logger.LogInformation("Documents created in {Elapsed}ms with {Count} element", timer.Elapsed, documents.Count);
    }

    public async Task CreateIndex(string indexName)
    {
        _logger.LogInformation("Creating Index");
        var timer = new Stopwatch();
        timer.Start();

        var createIndexResponse = await _client.Indices.CreateAsync(indexName, c => c.Map<LogDocument>(m => m.AutoMap<LogDocument>()));
        _logger.LogInformation("Index Created in {Elapsed}ms. Response: {@Response}", timer.Elapsed, createIndexResponse);
        timer.Stop();
    }
}