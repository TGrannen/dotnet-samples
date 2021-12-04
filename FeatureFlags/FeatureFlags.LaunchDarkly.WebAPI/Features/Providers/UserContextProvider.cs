using System.Threading.Tasks;
using FeatureFlags.LaunchDarkly.WebAPI.Services;
using FeatureFlags.Library.Core.Context;
using Microsoft.Extensions.Logging;

namespace FeatureFlags.LaunchDarkly.WebAPI.Features.Providers
{
    class UserContextProvider : IContextProvider
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserContextProvider> _logger;

        public UserContextProvider(IUserService userService,ILogger<UserContextProvider> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        public Task<IFeatureContext> GetUserAsync()
        {
            var user = _userService.GetUser();
            _logger.LogInformation("User Provided: {@User}", user);
            var context = new FeatureContext
            {
                Key = user.Id,
                Name = new ContextAttribute<string>(user.Name)
            };
            return Task.FromResult((IFeatureContext) context);
        }
    }
}