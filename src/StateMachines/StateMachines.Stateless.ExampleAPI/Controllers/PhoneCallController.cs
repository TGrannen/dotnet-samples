using Microsoft.AspNetCore.Mvc;
using StateMachines.Stateless.ExampleAPI.PhoneCall;
using StateMachines.Stateless.ExampleAPI.Services;

namespace StateMachines.Stateless.ExampleAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PhoneCallController(PhoneCallSm phoneCallSm, StateMachineExporter exporter, ILogger<PhoneCallController> logger)
    : ControllerBase
{
    private readonly ILogger<PhoneCallController> _logger = logger;

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            State = phoneCallSm.State.ToString()
        });
    }

    [HttpGet]
    [Route("Export")]
    public IActionResult Export()
    {
        return Ok(StateMachineExporter.ExportToJson(phoneCallSm.StateMachine));
    }

    [HttpGet]
    [Route("ExportImage")]
    public IActionResult ExportImage()
    {
        return File(exporter.ExportToImage(phoneCallSm.StateMachine), "Image/png");
    }

    [HttpGet]
    [Route("ExportImageV2")]
    public IActionResult ExportImageV2()
    {
        return File(exporter.ExportToImage(new PhoneCallSMV2().StateMachine), "Image/png");
    }

    [HttpPost]
    public IActionResult FireTrigger(PhoneTrigger trigger)
    {
        try
        {
            phoneCallSm.Fire(trigger);

            return Ok(new
            {
                State = phoneCallSm.State.ToString()
            });
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                State = phoneCallSm.State.ToString(),
                Exception = e.Message,
            });
        }
    }
}