using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StateMachines.Stateless.ExampleAPI.DynamicSM;
using StateMachines.Stateless.ExampleAPI.Services;

namespace StateMachines.Stateless.ExampleAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DynamicStateMachineController : ControllerBase
    {
        private readonly StateMachineExporter _exporter;
        private readonly DynamicStateMachineService _service;
        private readonly ILogger<DynamicStateMachineController> _logger;

        public DynamicStateMachineController(StateMachineExporter exporter,
            DynamicStateMachineService service,
            ILogger<DynamicStateMachineController> logger)
        {
            _exporter = exporter;
            _service = service;
            _logger = logger;
        }

        [HttpPost]
        [Route("Export")]
        public IActionResult Export(ModelBasedStateMachine setups)
        {
            var stateMachine = _service.CreateStateMachine(setups.Configurations, setups.InitialState);
            return Ok(_exporter.ExportToJson(stateMachine));
        }


        [HttpPost]
        [Route("ExportImage")]
        public IActionResult ExportImage(ModelBasedStateMachine setups)
        {
            var stateMachine = _service.CreateStateMachine(setups.Configurations, setups.InitialState);
            return File(_exporter.ExportToImage(stateMachine), "Image/png");
        }
        
        
        [HttpPost]
        [Route("ExportSvg")]
        public IActionResult ExportSvg([FromBody] ModelBasedStateMachine setups)
        {
            var stateMachine = _service.CreateStateMachine(setups.Configurations, setups.InitialState);
            return Ok(_exporter.ExportToSvg(stateMachine));
        }

        [HttpPost]
        [Route("FromStrings/Export")]
        public IActionResult Export(StringBasedStateMachine strings)
        {
            var setups = _service.ConvertFromStrings(strings.Configurations);
            var stateMachine = _service.CreateStateMachine(setups, strings.InitialState);
            return Ok(_exporter.ExportToJson(stateMachine));
        }


        [HttpPost]
        [Route("FromStrings/ExportImage")]
        public IActionResult ExportImage([FromBody] StringBasedStateMachine strings)
        {
            var setups = _service.ConvertFromStrings(strings.Configurations);
            var stateMachine = _service.CreateStateMachine(setups, strings.InitialState);
            return File(_exporter.ExportToImage(stateMachine), "Image/png");
        }

        [HttpPost]
        [Route("FromStrings/ExportSvg")]
        public IActionResult ExportSvg([FromBody] StringBasedStateMachine strings)
        {
            var setups = _service.ConvertFromStrings(strings.Configurations);
            var stateMachine = _service.CreateStateMachine(setups, strings.InitialState);
            return Ok(_exporter.ExportToSvg(stateMachine));
        }
    }

    public class ModelBasedStateMachine
    {
        public State InitialState { get; set; }
        public IEnumerable<StateSetup> Configurations { get; set; }
    }

    public class StringBasedStateMachine
    {
        public State InitialState { get; set; }

        /// <summary>
        /// Strings that denote how to configure the state machine
        /// </summary>
        /// <example>["State0,Trigger4,State1","State1,Trigger1,State3", "State3,Trigger0,State0", "State3,Trigger2,State4" ]</example>
        public IEnumerable<string> Configurations { get; set; }
    }
}