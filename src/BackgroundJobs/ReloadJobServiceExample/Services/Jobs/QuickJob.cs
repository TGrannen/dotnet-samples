using ReloadJobServiceExample.Testing;

namespace ReloadJobServiceExample.Services.Jobs;

public class QuickJob(ResultProvider resultProvider) : IReloadJob
{
    public async Task<bool> Execute(CancellationToken token)
    {
        await Task.Delay(TimeSpan.FromSeconds(1), token);

        return resultProvider.Result;
    }
}