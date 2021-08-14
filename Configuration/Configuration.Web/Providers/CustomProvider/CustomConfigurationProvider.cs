using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Configuration.Web.Providers.CustomProvider
{
    public class CustomConfigurationProvider : ConfigurationProvider
    {
        private string _dynamicValue;

        public CustomConfigurationProvider()
        {
            CustomConfigChangeObserverSingleton.Instance.Changed += CustomChangeObserver_Changed;
        }

        private void CustomChangeObserver_Changed(object sender, ConfigChangeEventArgs e)
        {
            _dynamicValue = e.DynamicValue;
            Load();
        }

        public override void Load()
        {
            if (_dynamicValue == null) return;

            Data = new Dictionary<string, string>
            {
                { "Settings3:DynamicValue", _dynamicValue },
            };
        }
    }
}