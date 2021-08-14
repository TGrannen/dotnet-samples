using Microsoft.Extensions.Configuration;

namespace Configuration.Web.Providers.CustomProvider
{
    public class CustomConfigurationSource : IConfigurationSource
    {
        public bool ReloadOnChange { get; set; }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new CustomConfigurationProvider();
        }
    }
}