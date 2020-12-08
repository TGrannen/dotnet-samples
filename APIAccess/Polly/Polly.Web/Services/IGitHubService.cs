using Polly.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Polly.Web.Services
{
    public interface IGitHubService
    {
        Task<IEnumerable<GitHubIssue>> GetAspNetDocsIssues();
    }
}