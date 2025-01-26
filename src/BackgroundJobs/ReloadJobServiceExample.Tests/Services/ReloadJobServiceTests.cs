using ReloadJobServiceExample.Services;
using ReloadJobServiceExample.Services.Jobs;

namespace ReloadJobServiceExample.Tests.Services;

public class ReloadJobServiceTests
{
    private readonly ReloadJobService<FakeJob> _sut;
    private readonly Mock<IReloadJob> _job = new();
    private readonly CancellationTokenSource _source = new();

    public ReloadJobServiceTests()
    {
        _job.Setup(x => x.Execute(It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var services = new ServiceCollection();
        services.AddSingleton(new FakeJob(_job.Object));
        var provider = services.BuildServiceProvider();
        _sut = new ReloadJobService<FakeJob>(provider, new NullLogger<ReloadJobService<FakeJob>>())
        {
            Delay = TimeSpan.Zero
        };
    }

    [Fact]
    public async Task ExecuteAsync_ShouldNotCallExecute_OnStartup()
    {
        await _sut.StartAsync(_source.Token);

        _source.CancelAfter(2);
        await _sut.ExecuteTask;

        _job.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCallExecute_WhenReloadCalled()
    {
        int callsCount = 0;
        await _sut.StartAsync(_source.Token);
        _job.Setup(x => x.Execute(It.IsAny<CancellationToken>())).Callback(() =>
        {
            callsCount++;
            if (2 >= callsCount) _source.Cancel();
        }).ReturnsAsync(true);

        await ExecuteWithReload();

        _job.Verify(x => x.Execute(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCallExecuteTwice_WhenTheFirstCallThrowsAnException()
    {
        var sequence = _job.SetupSequence(x => x.Execute(It.IsAny<CancellationToken>()));
        sequence.ThrowsAsync(new Exception("TEST"));
        sequence.ReturnsAsync(() =>
        {
            _source.Cancel();
            return true;
        });

        await ExecuteWithReload();

        _job.Verify(x => x.Execute(It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(100)]
    public async Task ExecuteAsync_ShouldCallExecuteUntilItSucceeds(int failureTimes)
    {
        var sequence = _job.SetupSequence(x => x.Execute(It.IsAny<CancellationToken>()));
        for (var i = 0; i < failureTimes; i++)
        {
            sequence.ReturnsAsync(false);
        }

        var cancelAfter = failureTimes + 1;
        for (var i = 0; i < cancelAfter; i++)
        {
            sequence.ReturnsAsync(() =>
            {
                _source.Cancel();
                return true;
            });
        }

        await ExecuteWithReload();

        _job.Verify(x => x.Execute(It.IsAny<CancellationToken>()), Times.Exactly(failureTimes + 1));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCallExecuteTwice_WhenStopHasBeenCalled()
    {
        _job.SetupSequence(x => x.Execute(It.IsAny<CancellationToken>()))
            .ReturnsAsync(false)
            .ReturnsAsync(() =>
            {
                _sut.Stop();
                _source.CancelAfter(100);
                return false;
            });

        await ExecuteWithReload();

        _job.Verify(x => x.Execute(It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    private async Task ExecuteWithReload(int millisecondsDelay = 10_000)
    {
        await _sut.StartAsync(_source.Token);
        _ = Task.Run(() => { _sut.Reload(); });
        _source.CancelAfter(millisecondsDelay);
        await _sut.ExecuteTask;
        await _sut.StopAsync(_source.Token);
    }

    private class FakeJob : IReloadJob
    {
        private readonly IReloadJob _reloadJob;

        public FakeJob(IReloadJob reloadJob)
        {
            _reloadJob = reloadJob;
        }

        public async Task<bool> Execute(CancellationToken token)
        {
            return await _reloadJob.Execute(token);
        }
    }
}