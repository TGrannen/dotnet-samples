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

    public async Task SeedData(int count, CancellationToken token = default)
    {
        var documents = _logsFaker.Generate(count);

        var timer = new Stopwatch();
        timer.Start();

        _logger.LogInformation("Writing Documents");
        int created = 0;
        foreach (var document in documents)
        {
            await _client.CreateDocumentAsync(new LogDocument
            {
                Body = document.Body.Trim()
            }, token);
            created++;
            if (token.IsCancellationRequested)
            {
                break;
            }
        }

        timer.Stop();
        _logger.LogInformation("{Count} Documents created in {Elapsed}ms", created, timer.Elapsed);
    }

    public async Task CreateIndex(string indexName)
    {
        _logger.LogInformation("Creating Index");
        var timer = new Stopwatch();
        timer.Start();

        var createIndexResponse = await _client.Indices.CreateAsync(indexName, c => c.Map<LogDocument>(m => m.AutoMap<LogDocument>()));
        _logger.LogInformation("Index Created in {Elapsed}ms. Successful: {@Response}", timer.Elapsed, createIndexResponse.ApiCall.Success);
        timer.Stop();
    }
}