namespace FlurlAPIAccess.TestWebAPI.Services;

public interface IFlurlRequestProvider
{
    IFlurlRequest GetAuthenticatedRequest();
}

internal class FlurlRequestProvider(IOptions<APIConfig> options, ILogger<FlurlRequestProvider> logger) : IFlurlRequestProvider
{
    public IFlurlRequest GetAuthenticatedRequest()
    {
        return options.Value.BaseUrl2
            .WithBasicAuth("TEST", "PASS")
            .ConfigureRequest(settings =>
            {
                settings.BeforeCall = call =>
                    logger.LogInformation("Calling Authenticated Endpoint  {Method} {CallRequest}", call.HttpRequestMessage.Method.Method,
                        call.Request.Url);
                settings.OnError = call =>
                    logger.LogError(call.Exception, "Call to Authenticated Endpoint failed: {CallRequest}", call.Request);
            });
    }
}