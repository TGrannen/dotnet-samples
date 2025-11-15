namespace WorkerServiceExample.Services;

public class HttpPollExample(ILogger<HttpPollExample> logger, IHttpClientFactory factory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var success = await IsGoogleUp();
                logger.Log(success
                        ? LogLevel.Information
                        : LogLevel.Error,
                    "Google is {Status}",
                    success ? "Online" : "Offline");
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Major Exception");
            }
            finally
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }

    private async Task<bool> IsGoogleUp()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync("http://google.com");
        bool success = response.IsSuccessStatusCode;
        return success;
    }
}