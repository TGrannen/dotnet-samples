using Configuration.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Configuration.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SettingsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IOptions<Settings1> _options;
        private readonly IOptionsSnapshot<Settings1> _optionsSnapshot;
        private readonly ISettings1 _settings1;

        public SettingsController(IConfiguration configuration,
            IOptions<Settings1> options,
            IOptionsSnapshot<Settings1> optionsSnapshot,
            ISettings1 settings1)
        {
            _configuration = configuration;
            _options = options;
            _optionsSnapshot = optionsSnapshot;
            _settings1 = settings1;
        }

        [HttpGet]
        public SettingResponse Get()
        {
            return new SettingResponse
            {
                Options = _options.Value,
                Snapshot = _optionsSnapshot.Value,
                Interface = _settings1,
                Value = _configuration.GetValue<string>("MySetting"),
                FileOverrideValue = _configuration.GetValue<string>("MySetting2"),
                DeepValue = _configuration.GetValue<string>("MySettingStructure:DeepValue"),
                EnvironmentValue = _configuration.GetValue<string>("MySetting3:EnvironmentVar"),
                CommandLineValue = _configuration.GetValue<string>("MySetting4"),
                SecretValue = _configuration.GetValue<string>("MySetting5"),
            };
        }
    }
}