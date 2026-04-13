using ApiService.Api.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ApiService.Api.Persistence;

public sealed class AuditingSaveChangesInterceptor(TimeProvider timeProvider) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateAuditableEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateAuditableEntities(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var now = timeProvider.GetUtcNow();

        foreach (var entry in context.ChangeTracker
                     .Entries<AuditableEntity>()
                     .Where(entry => entry.State == EntityState.Deleted))
        {
            entry.State = EntityState.Modified;
            entry.Entity.IsDeleted = true;
        }

        foreach (var entry in context.ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    entry.Entity.ModifiedAt = now;
                    ApplySoftDeleteTimestamps(entry.Entity, now);
                    break;
                case EntityState.Modified:
                    entry.Entity.ModifiedAt = now;
                    ApplySoftDeleteTimestamps(entry.Entity, now);
                    break;
                case EntityState.Detached:
                case EntityState.Unchanged:
                case EntityState.Deleted:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private static void ApplySoftDeleteTimestamps(AuditableEntity entity, DateTimeOffset now)
    {
        if (entity is { IsDeleted: true, DeletedAt: null })
        {
            entity.DeletedAt ??= now;
        }
    }
}
