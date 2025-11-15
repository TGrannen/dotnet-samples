using ReloadJobServiceExample.Testing;

namespace ReloadJobServiceExample.Services.Jobs;

public class NormalJob(ResultProvider resultProvider) : IReloadJob
{
    public async Task<bool> Execute(CancellationToken token)
    {
        await Task.Delay(TimeSpan.FromSeconds(4), token);

        return resultProvider.Result;
    }
}