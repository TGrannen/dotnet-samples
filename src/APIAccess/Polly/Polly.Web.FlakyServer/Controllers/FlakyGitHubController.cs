using Microsoft.AspNetCore.Mvc;
using Polly.Shared.Models;
using System.Globalization;

namespace Polly.Web.FlakyServer.Controllers;

[ApiController]
[Route("[controller]")]
public class FlakyGitHubController : ControllerBase
{
    private static readonly Random Random = new();

    [HttpGet]
    public async Task<IActionResult> GetAspNetDocsIssues()
    {
        if (Random.Next(1, 3) == 1)
        {
            await Task.Delay(500);
            return StatusCode(500);
        }

        var issues = Issues();
        return issues != null ? (IActionResult)Ok(issues) : NotFound();
    }

    private static List<GitHubIssue> Issues()
    {
        return new List<GitHubIssue>
        {
            new()
            {
                id = 757783375,
                node_id = "MDExOlB1bGxSZXF1ZXN0NTMzMDk0NDc3",
                number = 20876,
                state = "open",
                title = "Update intro.md",
                body = "Fixes #20875\r\n@the-wazz please review",
                locked = false,
                comments = 0,
                closed_at = null,
                created_at = DateTime.Parse("2020/12/05 21:25:26.0000000", CultureInfo.InvariantCulture),
                updated_at = DateTime.Parse("2020/12/05 21:26:06.0000000", CultureInfo.InvariantCulture)
            },
            new()
            {
                id = 757748085,
                node_id = "MDExOlB1bGxSZXF1ZXN0NTMzMDY5MzU5",
                number = 20874,
                state = "open",
                title = "Updates to Routing",
                body = "Improved the Routing docs",
                locked = false,
                comments = 0,
                closed_at = null,
                created_at = DateTime.Parse("2020/12/05 17:57:24.0000000", CultureInfo.InvariantCulture),
                updated_at = DateTime.Parse("2020/12/05 18:50:08.0000000", CultureInfo.InvariantCulture)
            }
        };
    }
}