using Microsoft.AspNetCore.Mvc;
using StateMachines.Stateless.ExampleAPI.DynamicSM;
using StateMachines.Stateless.ExampleAPI.Services;

namespace StateMachines.Stateless.ExampleAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class DynamicStateMachineController(StateMachineExporter exporter, DynamicStateMachineService service) : ControllerBase
{
    [HttpPost]
    [Route("Export")]
    public IActionResult Export(ModelBasedStateMachine setups)
    {
        var stateMachine = service.CreateStateMachine(setups.Configurations, setups.InitialState);
        return Ok(StateMachineExporter.ExportToJson(stateMachine));
    }

    [HttpPost]
    [Route("ExportImage")]
    public IActionResult ExportImage(ModelBasedStateMachine setups)
    {
        var stateMachine = service.CreateStateMachine(setups.Configurations, setups.InitialState);
        return File(exporter.ExportToImage(stateMachine), "Image/png");
    }

    [HttpPost]
    [Route("ExportSvg")]
    public IActionResult ExportSvg([FromBody] ModelBasedStateMachine setups)
    {
        var stateMachine = service.CreateStateMachine(setups.Configurations, setups.InitialState);
        return Ok(exporter.ExportToSvg(stateMachine));
    }

    [HttpPost]
    [Route("FromStrings/Export")]
    public IActionResult Export(StringBasedStateMachine strings)
    {
        var setups = service.ConvertFromStrings(strings.Configurations);
        var stateMachine = service.CreateStateMachine(setups, strings.InitialState);
        return Ok(StateMachineExporter.ExportToJson(stateMachine));
    }

    [HttpPost]
    [Route("FromStrings/ExportImage")]
    public IActionResult ExportImage([FromBody] StringBasedStateMachine strings)
    {
        var setups = service.ConvertFromStrings(strings.Configurations);
        var stateMachine = service.CreateStateMachine(setups, strings.InitialState);
        return File(exporter.ExportToImage(stateMachine), "Image/png");
    }

    [HttpPost]
    [Route("FromStrings/ExportSvg")]
    public IActionResult ExportSvg([FromBody] StringBasedStateMachine strings)
    {
        var setups = service.ConvertFromStrings(strings.Configurations);
        var stateMachine = service.CreateStateMachine(setups, strings.InitialState);
        return Ok(exporter.ExportToSvg(stateMachine));
    }
}

public class ModelBasedStateMachine
{
    public State InitialState { get; set; }
    public IEnumerable<StateSetup> Configurations { get; set; } = null!;
}

public class StringBasedStateMachine
{
    public State InitialState { get; set; }

    /// <summary>
    /// Strings that denote how to configure the state machine
    /// </summary>
    /// <example>["State0,Trigger4,State1","State1,Trigger1,State3", "State3,Trigger0,State0", "State3,Trigger2,State4" ]</example>
    public IEnumerable<string> Configurations { get; set; } = null!;
}