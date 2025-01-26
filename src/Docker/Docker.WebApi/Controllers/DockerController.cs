using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Extensions;
using Ductus.FluentDocker.Model.Common;
using Ductus.FluentDocker.Services;
using Microsoft.AspNetCore.Mvc;

namespace Docker.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class DockerController : ControllerBase
{
    private readonly ILogger<DockerController> _logger;

    public DockerController(ILogger<DockerController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    [Route("RunNginx")]
    public async Task<IActionResult> RunNginx()
    {
        var image = "nginx";
        _logger.LogInformation("Starting up {Image} container", image);
        using var container =
            new Builder().UseContainer()
                .UseImage(image)
                .ExposePort(80)
                .WithEnvironment("POSTGRES_PASSWORD=mysecretpassword")
                .WaitForPort("80/tcp", 30000 /*30s*/)
                .WithName("Fancy-Container")
                .Build()
                .Start();

        _logger.LogInformation("{Image} container started", image);
        var config = container.GetConfiguration(true);
        if (config.State.ToServiceState() == ServiceRunningState.Running)
        {
            await Delay(5000);

            _logger.LogInformation("Stopping {Image} container", image);
            return Ok();
        }

        _logger.LogInformation("Stopping {Image} container", image);
        return BadRequest();
    }

    [HttpPost]
    [Route("RunDockerCompose")]
    public async Task<IActionResult> RunDockerCompose()
    {
        _logger.LogInformation("Starting up {Image}", "Docker Compose");
        var file = Path.Combine(Directory.GetCurrentDirectory(),
            (TemplateString) "Resources/WordPress/docker-compose.yml");

        using var svc = new Builder()
            .UseContainer()
            .UseCompose()
            .FromFile(file)
            .RemoveOrphans()
            .WaitForHttp("wordpress", "http://localhost:8085/wp-admin/install.php")
            .Build().Start();

        // We now have a running WordPress with a MySql database        
        var installPage = await "http://localhost:8085/wp-admin/install.php".Wget();
        var success = installPage.IndexOf("https://wordpress.org/", StringComparison.Ordinal) != -1;

        await Delay(5000);

        _logger.LogInformation("Stopping {Image}", "Docker Compose");
        return success ? Ok() : BadRequest();
    }

    private async Task Delay(int millisecondsDelay)
    {
        _logger.LogInformation("Waiting for {Milliseconds} ms", millisecondsDelay);
        await Task.Delay(millisecondsDelay);
        _logger.LogInformation("Done Waiting");
    }
}