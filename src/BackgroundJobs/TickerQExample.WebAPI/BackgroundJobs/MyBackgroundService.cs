using TickerQ.Utilities.Base;
using TickerQ.Utilities.Models;

namespace TickerQExample.WebAPI.BackgroundJobs;

public class MyBackgroundService(ILogger<MyBackgroundService> logger)
{
    [TickerFunction("ExampleMethod", "*/5 * * * *")]
    public void ExampleMethod()
    {
        // Your background job logic goes here...
        logger.LogInformation("Ran {MethodName}", nameof(ExampleMethod));
    }

    [TickerFunction("DeactivateStaleUsers", "0 0 * * 0")]
    public async Task DeactivateStaleUsersAsync()
    {
        logger.LogInformation("Running {MethodName}", nameof(DeactivateStaleUsersAsync));
        await Task.Delay(TimeSpan.FromSeconds(2));
        logger.LogInformation("Ran {MethodName}", nameof(DeactivateStaleUsersAsync));
    }

    [TickerFunction("CleanUpUserSessions", "0 */2 * * *")]
    public void CleanUpUserSessions()
    {
        logger.LogInformation("Ran {MethodName}", nameof(CleanUpUserSessions));
    }

    [TickerFunction("WithObject")]
    public void WithObject(TickerFunctionContext<TestObject> tickerFunctionContext)
    {
        logger.LogInformation("Ran {MethodName} {@Data}", nameof(WithObject), tickerFunctionContext.Request);
    }

}