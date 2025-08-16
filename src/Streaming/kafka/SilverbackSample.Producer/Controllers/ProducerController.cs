using Microsoft.AspNetCore.Mvc;
using Silverback.Messaging.Broker;
using Silverback.Messaging.Publishing;

namespace SilverbackSample.Producer.Controllers;

[ApiController]
[Route("[controller]")]
public class ProducerController(IPublisher publisher, IBroker broker, ILogger<ProducerController> logger, TimeProvider timeProvider) : ControllerBase
{
    [HttpGet]
    [Route("IsConnected")]
    public string IsConnected()
    {
        return broker.IsConnected ? "Connected" : "Disconnected";
    }

    [HttpPost]
    [Route("Produce")]
    public async Task<IActionResult> ProduceS(int number = 4561)
    {
        var sampleMessage = new SampleMessage { Number = number, UtcNow = timeProvider.GetUtcNow() };
        await publisher.PublishAsync(sampleMessage);

        logger.LogInformation("Produced {Number}", number);

        return Ok(sampleMessage);
    }

    [HttpPost]
    [Route("Produce/Batch")]
    public async Task<IActionResult> ProduceBatch(int count = 20)
    {
        var sampleMessages = Enumerable.Range(0, count).Select(x => new SampleBatchMessage { Number = x, UtcNow = timeProvider.GetUtcNow() }).ToList();
        foreach (var sampleMessage in sampleMessages)
        {
            await publisher.PublishAsync(sampleMessage);
        }

        logger.LogInformation("Produced {Number} messages", sampleMessages.Count);

        return Ok(sampleMessages);
    }
}