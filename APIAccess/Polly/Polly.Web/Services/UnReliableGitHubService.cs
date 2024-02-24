using Polly.Shared.Models;
using System.Text.Json;

namespace Polly.Web.Services;

public interface IFlakyGitHubService
{
    Task<IEnumerable<GitHubIssue>> GetAspNetDocsIssues();
}

public class UnReliableGitHubService : IFlakyGitHubService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UnReliableGitHubService> _logger;

    public UnReliableGitHubService(HttpClient httpClient, ILogger<UnReliableGitHubService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IEnumerable<GitHubIssue>> GetAspNetDocsIssues()
    {
        _logger.LogInformation("Getting From Flaky Server");
        HttpResponseMessage response;
        try
        {
            response = await _httpClient.GetAsync("FlakyGitHub");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error getting data from Flaky Server");
            return null;
        }

        response.EnsureSuccessStatusCode();

        await using var responseStream = await response.Content.ReadAsStreamAsync();
        var issues = (await JsonSerializer.DeserializeAsync<IEnumerable<GitHubIssue>>(responseStream))?.Take(3).ToList();

        _logger.LogInformation("Retrieved {Count} Github Issue Information", issues?.Count ?? 0);
        return issues;
    }
}