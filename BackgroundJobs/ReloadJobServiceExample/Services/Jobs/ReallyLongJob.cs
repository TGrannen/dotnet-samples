using ReloadJobServiceExample.Testing;

namespace ReloadJobServiceExample.Services.Jobs;

public class ReallyLongJob : IReloadJob
{
    private readonly ResultProvider _resultProvider;

    public ReallyLongJob(ResultProvider resultProvider)
    {
        _resultProvider = resultProvider;
    }

    public async Task<bool> Execute()
    {
        await Task.Delay(TimeSpan.FromSeconds(15));

        return _resultProvider.Result;
    }
}