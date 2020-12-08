using Microsoft.Extensions.Logging;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;
using Polly.Retry;
using Polly.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Polly.Web.Services
{
    public class GitHubService : IGitHubService
    {
        private const int MaxRetries = 3;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<GitHubService> _logger;
        private static readonly Random Random = new Random();
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

        public GitHubService(IHttpClientFactory httpClientFactory, ILogger<GitHubService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;

            var delay = Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), MaxRetries);

            _retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(delay,
                    onRetry: (exception, duration, retryCount, context) =>
                    {
                        _logger.LogWarning("Retry Number: {RetryCount}  Waiting: {Duration:#}ms, due to: {Message}.", retryCount, duration.Milliseconds, exception.Exception.Message);
                    }
                );
        }

        public async Task<IEnumerable<GitHubIssue>> GetAspNetDocsIssues()
        {
            PrintWhitespace();
            _logger.LogInformation("Getting Github Issue Information");

            var client = _httpClientFactory.CreateClient("GitHub");

            var message = await _retryPolicy.ExecuteAsync(async () =>
            {
                if (Random.Next(1, 3) == 1)
                {
                    await Task.Delay(500);
                    throw new HttpRequestException("This is a fake request exception");
                }

                var response = await client.GetAsync("/repos/aspnet/AspNetCore.Docs/issues?state=open&sort=created&direction=desc");

                response.EnsureSuccessStatusCode();

                return response;
            });

            await using var responseStream = await message.Content.ReadAsStreamAsync();
            var issues = (await JsonSerializer.DeserializeAsync<IEnumerable<GitHubIssue>>(responseStream))?.Take(3).ToList();
            _logger.LogInformation("Retrieved {Count} Github Issue Information", issues?.Count() ?? 0);
            PrintWhitespace();
            return issues;
        }

        private static void PrintWhitespace()
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}