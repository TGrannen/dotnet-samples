using System.Diagnostics;
using Bogus;

namespace FullTextSearch.ElasticSearch.Web.Services;

public class SeedService(ElasticClient client, ILogger<SeedService> logger)
{
    public async Task SeedData(int count, CancellationToken token = default)
    {
        var faker = new Faker<LogDocument>()
            .RuleFor(o => o.Id, Guid.NewGuid)
            .RuleFor(o => o.Body, f => f.Random.Words());

        var documents = faker.Generate(count);

        var timer = new Stopwatch();
        timer.Start();

        logger.LogInformation("Writing Documents");
        var createdCount = 0;

        foreach (var document in documents)
        {
            await client.CreateDocumentAsync(new LogDocument
            {
                Body = document.Body.Trim()
            }, token);
            createdCount++;

            if (token.IsCancellationRequested)
            {
                break;
            }
        }

        timer.Stop();
        logger.LogInformation("{Count} Documents created in {Elapsed}ms", createdCount, timer.Elapsed);
    }

    public async Task CreateIndex(string indexName)
    {
        logger.LogInformation("Creating Index");

        var timer = new Stopwatch();
        timer.Start();
        var createIndexResponse = await client.Indices.CreateAsync(indexName, c => c.Map<LogDocument>(m => m.AutoMap<LogDocument>()));
        timer.Stop();

        logger.LogInformation("Index Created in {Elapsed}ms. Successful: {@Response}", timer.Elapsed, createIndexResponse.ApiCall.Success);
    }
}