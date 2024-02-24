using Polly.Shared.Models;
using System.Text.Json;

namespace Polly.Web.Services;

public interface IGitHubService
{
    Task<IEnumerable<GitHubIssue>> GetAspNetDocsIssues();
}

public class GitHubService : IGitHubService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<GitHubService> _logger;
    private static readonly Random Random = new();
    private readonly ResiliencePipeline<HttpResponseMessage> _pipeline;

    public GitHubService(IHttpClientFactory httpClientFactory, ResiliencePipelineProvider<string> pipelineProvider,
        ILogger<GitHubService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _pipeline = pipelineProvider.GetPipeline<HttpResponseMessage>(ResiliencePipelines.GithubRetry);
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

            var response = await client.GetAsync("/repos/aspnet/AspNetCore.Docs/issues?state=open&sort=created&direction=desc",
                cancellationToken);
            response.EnsureSuccessStatusCode();
            return response;
        });

        await using var responseStream = await message.Content.ReadAsStreamAsync();
        var issues = (await JsonSerializer.DeserializeAsync<IEnumerable<GitHubIssue>>(responseStream))?.Take(3).ToList();
        _logger.LogInformation("Retrieved {Count} Github Issue Information", issues?.Count ?? 0);

        return issues;
    }
}