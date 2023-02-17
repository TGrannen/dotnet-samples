using Configuration.Web.Models;
using Configuration.Web.Providers.CustomProvider;
using Microsoft.AspNetCore.Mvc;

namespace Configuration.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class SettingsController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IOptions<Settings1> _options;
    private readonly IOptions<Settings2> _options2;
    private readonly IOptionsSnapshot<Settings1> _optionsSnapshot;
    private readonly IOptionsSnapshot<Settings3> _optionsSnapshot3;

    public SettingsController(IConfiguration configuration,
        IOptions<Settings1> options,
        IOptionsSnapshot<Settings1> optionsSnapshot,
        IOptions<Settings2> options2,
        IOptionsSnapshot<Settings3> optionsSnapshot3)
    {
        _configuration = configuration;
        _options = options;
        _optionsSnapshot = optionsSnapshot;
        _options2 = options2;
        _optionsSnapshot3 = optionsSnapshot3;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            Settings1 = new
            {
                Options = _options.Value,
                Snapshot = _optionsSnapshot.Value
            },
            Settings2 = _options2.Value,
            Settings3 = _optionsSnapshot3.Value,
            Value = _configuration.GetValue<string>("MySetting"),
            FileOverrideValue = _configuration.GetValue<string>("MySetting2"),
            DeepValue = _configuration.GetValue<string>("MySettingStructure:DeepValue"),
            EnvironmentValue = _configuration.GetValue<string>("MySetting3:EnvironmentVar"),
            CommandLineValue = _configuration.GetValue<string>("MySetting4"),
            SecretValue = _configuration.GetValue<string>("MySetting5"),
        });
    }

    [HttpPost]
    public IActionResult UpdateDynamicValue([FromQuery] string value)
    {
        CustomConfigChangeObserverSingleton.Instance.OnChanged(new ConfigChangeEventArgs { DynamicValue = value });
        return Ok();
    }
}