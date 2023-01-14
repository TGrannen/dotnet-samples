using Microsoft.Extensions.Logging;

namespace FeatureFlags.Library.Flagsmith.Internal;

internal class EnvFlagsProvider : IEnvFlagsProvider
{
    private readonly FlagsmithClient _client;
    private readonly ILogger<EnvFlagsProvider> _logger;
    private Flags? _flags;

    public EnvFlagsProvider(FlagsmithClient client, ILogger<EnvFlagsProvider> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<Flags> GetFlags()
    {
        if (_flags != null)
        {
            return _flags;
        }

        _flags = await _client.GetEnvironmentFlags();
        _logger.LogInformation("Env flags read in");
        return _flags;
    }
}