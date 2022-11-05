using ReloadJobServiceExample.Testing;

namespace ReloadJobServiceExample.Services.Jobs;

public class ReallyLongJob : IReloadJob
{
    private readonly ResultProvider _resultProvider;

    public ReallyLongJob(ResultProvider resultProvider)
    {
        _resultProvider = resultProvider;
    }

    public async Task<bool> Execute(CancellationToken token)
    {
        await Task.Delay(TimeSpan.FromSeconds(15), token);

        return _resultProvider.Result;
    }
}