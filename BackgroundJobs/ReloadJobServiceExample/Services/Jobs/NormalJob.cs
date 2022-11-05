using ReloadJobServiceExample.Testing;

namespace ReloadJobServiceExample.Services.Jobs;

public class NormalJob : IReloadJob
{
    private readonly ResultProvider _resultProvider;

    public NormalJob(ResultProvider resultProvider)
    {
        _resultProvider = resultProvider;
    }

    public async Task<bool> Execute(CancellationToken token)
    {
        await Task.Delay(TimeSpan.FromSeconds(4), token);

        return _resultProvider.Result;
    }
}