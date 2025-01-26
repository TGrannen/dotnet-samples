using Microsoft.AspNetCore.Mvc;
using StateMachines.Stateless.ExampleAPI.PhoneCall;
using StateMachines.Stateless.ExampleAPI.Services;

namespace StateMachines.Stateless.ExampleAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PhoneCallController : ControllerBase
{
    private readonly PhoneCallSm _phoneCallSm;
    private readonly StateMachineExporter _exporter;
    private readonly ILogger<PhoneCallController> _logger;

    public PhoneCallController(PhoneCallSm phoneCallSm, StateMachineExporter exporter, ILogger<PhoneCallController> logger)
    {
        _phoneCallSm = phoneCallSm;
        _exporter = exporter;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(SerializeState());
    }

    [HttpGet]
    [Route("Export")]
    public IActionResult Export()
    {
        return Ok(_exporter.ExportToJson(_phoneCallSm.StateMachine));
    }

    [HttpGet]
    [Route("ExportImage")]
    public IActionResult ExportImage()
    {
        return File(_exporter.ExportToImage(_phoneCallSm.StateMachine), "Image/png");
    }
        
    [HttpGet]
    [Route("ExportImageV2")]
    public IActionResult ExportImageV2()
    {
        return File(_exporter.ExportToImage(new PhoneCallSMV2().StateMachine), "Image/png");
    }

    [HttpPost]
    public IActionResult FireTrigger(PhoneTrigger trigger)
    {
        try
        {
            _phoneCallSm.Fire(trigger);

            return Ok(SerializeState());
        }
        catch (Exception e)
        {
            return BadRequest(SerializeState(e));
        }
    }

    private object SerializeState(Exception exception = null)
    {
        return new
        {
            State = _phoneCallSm.State.ToString(),
            Exception = exception?.Message,
        };
    }
}