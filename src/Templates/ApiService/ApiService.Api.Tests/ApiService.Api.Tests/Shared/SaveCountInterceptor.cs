using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ApiService.Api.Tests.Shared;

public class SaveChangesTracker
{
    public int Count { get; set; }
}

public class SaveCountInterceptor(SaveChangesTracker tracker) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        tracker.Count++;
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        tracker.Count++;
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
