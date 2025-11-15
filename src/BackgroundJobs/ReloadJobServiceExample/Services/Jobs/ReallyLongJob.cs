using ReloadJobServiceExample.Testing;

namespace ReloadJobServiceExample.Services.Jobs;

public class ReallyLongJob(ResultProvider resultProvider) : IReloadJob
{
    public async Task<bool> Execute(CancellationToken token)
    {
        await Task.Delay(TimeSpan.FromSeconds(15), token);

        return resultProvider.Result;
    }
}