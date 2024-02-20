using Polly.Retry;
using Polly.Shared.Models;
using System.Text.Json;
using Serilog;

namespace Polly.Web.Services;

public class GitHubService : IGitHubService
{
    private const int MaxRetries = 3;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<GitHubService> _logger;
    private static readonly Random Random = new Random();
    private readonly ResiliencePipeline<HttpResponseMessage> _pipeline;

    public GitHubService(IHttpClientFactory httpClientFactory, ILogger<GitHubService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;

        _pipeline = new ResiliencePipelineBuilder<HttpResponseMessage>()
            .AddRetry(new RetryStrategyOptions<HttpResponseMessage>
            {
                ShouldHandle = args => args switch
                {
                    { Outcome: { Exception: HttpRequestException } } => PredicateResult.True(),
                    { Outcome: { Exception: InvalidOperationException } } => PredicateResult.True(),
                    { Outcome: { Result: { IsSuccessStatusCode: false } } } => PredicateResult.True(),
                    _ => PredicateResult.False()
                },
                MaxRetryAttempts = MaxRetries,
                Delay = TimeSpan.FromSeconds(2),
                BackoffType = DelayBackoffType.Exponential,
                OnRetry = static args =>
                {
                    Log.Warning("Retry Number: {RetryCount}  Waiting: {Duration}, due to: {Message}", args.AttemptNumber, args.RetryDelay, args.Outcome.Exception?.Message);
                    return ValueTask.CompletedTask;
                }
            }).Build();
    }

    public async Task<IEnumerable<GitHubIssue>> GetAspNetDocsIssues()
    {
        _logger.LogInformation("Getting Github Issue Information");

        var client = _httpClientFactory.CreateClient("GitHub");

        var message = await _pipeline.ExecuteAsync(async (cancellationToken) =>
        {
            if (Random.Next(1, 3) == 1)
            {
                await Task.Delay(500, cancellationToken);
                throw new HttpRequestException("This is a fake request exception");
            }

            var response = await client.GetAsync("/repos/aspnet/AspNetCore.Docs/issues?state=open&sort=created&direction=desc", cancellationToken);

            response.EnsureSuccessStatusCode();

            return response;
        });

        await using var responseStream = await message.Content.ReadAsStreamAsync();
        var issues = (await JsonSerializer.DeserializeAsync<IEnumerable<GitHubIssue>>(responseStream))?.Take(3).ToList();
        _logger.LogInformation("Retrieved {Count} Github Issue Information", issues?.Count ?? 0);

        return issues;
    }
}