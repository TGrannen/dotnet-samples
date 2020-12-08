using System.Collections.Generic;
using System.Threading.Tasks;
using Polly.Shared.Models;

namespace Polly.Web.Services
{
    public interface IFlakyGitHubService
    {
        Task<IEnumerable<GitHubIssue>> GetAspNetDocsIssues();
    }
}