using Microsoft.AspNetCore.Mvc;
using Polly.Web.Services;
using System.Threading.Tasks;

namespace Polly.Web.Controllers
{
    [ApiController]
    public class GithubController : ControllerBase
    {
        private readonly IGitHubService _githubService;
        private readonly IFlakyGitHubService _flakyGitHubService;

        public GithubController(IGitHubService githubService, IFlakyGitHubService flakyGitHubService)
        {
            _githubService = githubService;
            _flakyGitHubService = flakyGitHubService;
        }

        [HttpGet("GetAspNetDocsIssues")]
        public async Task<IActionResult> GetAspNetDocsIssues()
        {
            var issues = await _githubService.GetAspNetDocsIssues();
            return issues != null ? (IActionResult)Ok(issues) : NotFound();
        }

        [HttpGet("GetFromFlaky")]
        public async Task<IActionResult> GetFromFlaky()
        {
            var issues = await _flakyGitHubService.GetAspNetDocsIssues();
            return issues != null ? (IActionResult)Ok(issues) : NotFound();
        }
    }
}