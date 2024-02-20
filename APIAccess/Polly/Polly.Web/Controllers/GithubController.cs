using Microsoft.AspNetCore.Mvc;

namespace Polly.Web.Controllers;

[ApiController]
public class GithubController(IGitHubService githubService, IFlakyGitHubService flakyGitHubService) : ControllerBase
{
    [HttpGet("GetAspNetDocsIssues")]
    public async Task<IActionResult> GetAspNetDocsIssues()
    {
        var issues = await githubService.GetAspNetDocsIssues();
        return issues != null ? (IActionResult)Ok(issues) : NotFound();
    }

    [HttpGet("GetFromFlaky")]
    public async Task<IActionResult> GetFromFlaky()
    {
        var issues = await flakyGitHubService.GetAspNetDocsIssues();
        return issues != null ? (IActionResult)Ok(issues) : NotFound();
    }
}