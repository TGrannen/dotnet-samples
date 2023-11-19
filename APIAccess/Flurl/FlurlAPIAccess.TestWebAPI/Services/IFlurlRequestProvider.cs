namespace FlurlAPIAccess.TestWebAPI.Services;

public interface IFlurlRequestProvider
{
    IFlurlRequest GetAuthenticatedRequest();
}

internal class FlurlRequestProvider : IFlurlRequestProvider
{
    private readonly IOptions<APIConfig> _options;
    private readonly ILogger _logger;

    public FlurlRequestProvider(IOptions<APIConfig> options, ILogger<FlurlRequestProvider> logger)
    {
        _options = options;
        _logger = logger;
    }

    public IFlurlRequest GetAuthenticatedRequest()
    {
        return _options.Value.BaseUrl2
            .WithBasicAuth("TEST", "PASS")
            .ConfigureRequest(settings =>
            {
                settings.BeforeCall = call =>
                    _logger.LogInformation("Calling Authenticated Endpoint  {Method} {CallRequest}", call.HttpRequestMessage.Method.Method,
                        call.Request.Url);
                settings.OnError = call =>
                    _logger.LogError(call.Exception, "Call to Authenticated Endpoint failed: {CallRequest}", call.Request);
            });
    }
}