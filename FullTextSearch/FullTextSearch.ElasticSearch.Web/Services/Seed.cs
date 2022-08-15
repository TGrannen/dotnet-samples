using System.Diagnostics;
using Bogus;
using Nest;

namespace FullTextSearch.ElasticSearch.Web.Services;

public class Seed
{
    private readonly ElasticClient _client;
    private readonly ILogger<Seed> _logger;

    public Seed(ElasticClient client, ILogger<Seed> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task SeedData(string indexName)
    {
        Randomizer.Seed = new Random(123456);
        var logsFaker = new Faker<LogDocument>()
            .RuleFor(o => o.Id, Guid.NewGuid)
            .RuleFor(o => o.Body, f => f.Random.Words());
        await CreateIndex(logsFaker.Generate(100), indexName);
    }

    async Task CreateIndex(List<LogDocument> documents, string indexName)
    {
        _logger.LogInformation("Creating Index");
        var timer = new Stopwatch();
        timer.Start();

        var createIndexResponse = await _client.Indices.CreateAsync(indexName, c => c.Map<LogDocument>(m => m.AutoMap<LogDocument>()));

        _logger.LogInformation("Writing Documents");
        foreach (var document in documents)
        {
            await _client.CreateDocumentAsync(new LogDocument()
            {
                Body = document.Body.Trim()
            });
        }

        timer.Stop();
        _logger.LogInformation("Index created in {Elapsed}ms with {Count} element", timer.Elapsed, documents.Count);
    }
}