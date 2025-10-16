using InventorySys.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace InventorySys.Infrastructure.Data.Interceptors;

public sealed class SoftDeleteInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        HandleSoftDeletableEnrties(eventData.Context);

        return base.SavingChanges(eventData, result);
    }


    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        HandleSoftDeletableEnrties(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public void HandleSoftDeletableEnrties(DbContext? context)
    {
        if (context == null) return;

        foreach (var softDeletableEntry in context.ChangeTracker.Entries<ISoftDeletable>().Where(e => e.State == EntityState.Deleted))
        {
            softDeletableEntry.State = EntityState.Modified;
            softDeletableEntry.Entity.IsDeleted = true;
        }
        
    }
}
