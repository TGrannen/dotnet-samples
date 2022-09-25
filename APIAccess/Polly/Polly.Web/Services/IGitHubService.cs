using Polly.Shared.Models;

namespace Polly.Web.Services;

public interface IGitHubService
{
    Task<IEnumerable<GitHubIssue>> GetAspNetDocsIssues();
}