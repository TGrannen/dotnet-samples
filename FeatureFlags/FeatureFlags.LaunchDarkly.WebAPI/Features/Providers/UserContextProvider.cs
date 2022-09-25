using FeatureFlags.LaunchDarkly.WebAPI.Services;
using FeatureFlags.Library.Core.Context;

namespace FeatureFlags.LaunchDarkly.WebAPI.Features.Providers;

class UserContextProvider : IContextProvider
{
    private readonly IUserService _userService;
    private readonly ILogger<UserContextProvider> _logger;
    private TestUser _user;

    public UserContextProvider(IUserService userService, ILogger<UserContextProvider> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public Task<IFeatureContext> GetUserAsync()
    {
        _user ??= _userService.GetUser();
            
        _logger.LogInformation("User Provided: {@User}", _user);
        var context = new FeatureContext
        {
            Key = _user.Id,
            Name = new ContextAttribute<string>(_user.Name)
        };
        return Task.FromResult((IFeatureContext) context);
    }
}