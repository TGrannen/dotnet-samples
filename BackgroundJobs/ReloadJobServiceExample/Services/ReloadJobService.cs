using ReloadJobServiceExample.Services.Jobs;
using Stateless;

namespace ReloadJobServiceExample.Services;

public interface IReloadJobService
{
    void SetReload();
}

class ReloadJobService<T> : BackgroundService, IReloadJobService where T : IReloadJob
{
    public TimeSpan Delay { get; init; } = TimeSpan.FromSeconds(2);
    private readonly IServiceProvider _provider;
    private readonly ILogger<ReloadJobService<T>> _logger;
    private CancellationTokenSource _childCts = new();
    private CancellationToken _stoppingToken;
    private readonly StateMachine<State, Trigger> _stateMachine;

    public ReloadJobService(IServiceProvider provider, ILogger<ReloadJobService<T>> logger)
    {
        _provider = provider;
        _logger = logger;
        _stateMachine = new StateMachine<State, Trigger>(State.Idle);
        _stateMachine.OnTransitioned(t => _logger.LogInformation("Job {Name} State Change {@Value}", typeof(T).Name, t));

        _stateMachine
            .Configure(State.Idle)
            .Permit(Trigger.Reload, State.Loading);

        _stateMachine
            .Configure(State.Loading)
            .Permit(Trigger.Successful, State.Loaded)
            .PermitReentry(Trigger.Unsuccessful)
            .Ignore(Trigger.Reload)
            .OnEntry(t =>
            {
                if (!t.IsReentry)
                {
                    _childCts.Cancel();
                }

                if (t.Trigger == Trigger.Unsuccessful)
                {
                    _childCts.CancelAfter((int)Delay.TotalMilliseconds);
                }
            })
            .OnExit(() => _childCts = CancellationTokenSource.CreateLinkedTokenSource(_stoppingToken))
            ;

        _stateMachine
            .Configure(State.Loaded)
            .Permit(Trigger.Reload, State.Loading)
            ;
    }


    public void SetReload()
    {
        if (_stateMachine.CanFire(Trigger.Reload))
        {
            _stateMachine.Fire(Trigger.Reload);
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _stoppingToken = stoppingToken;
        _childCts = CancellationTokenSource.CreateLinkedTokenSource(_stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            if (_stateMachine.IsInState(State.Loading))
            {
                var trigger = await RunJob(_provider.GetRequiredService<T>()) ? Trigger.Successful : Trigger.Unsuccessful;
                if (_stateMachine.CanFire(trigger))
                {
                    await _stateMachine.FireAsync(trigger);
                }
            }

            try
            {
                await Task.Delay(TimeSpan.FromDays(30), _childCts.Token);
            }
            catch (TaskCanceledException)
            {
            }
        }
    }

    private async Task<bool> RunJob(T job)
    {
        try
        {
            return await job.Execute();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error during {Name} job execution", typeof(T).Name);
            return false;
        }
    }

    private enum State
    {
        Idle,
        Loading,
        Loaded
    }

    private enum Trigger
    {
        Reload,
        Unsuccessful,
        Successful,
    }
}