using ReloadJobServiceExample.Testing;

namespace ReloadJobServiceExample.Services.Jobs;

public class QuickJob : IReloadJob
{
    private readonly ResultProvider _resultProvider;

    public QuickJob(ResultProvider resultProvider)
    {
        _resultProvider = resultProvider;
    }

    public async Task<bool> Execute(CancellationToken token)
    {
        await Task.Delay(TimeSpan.FromSeconds(1), token);

        return _resultProvider.Result;
    }
}