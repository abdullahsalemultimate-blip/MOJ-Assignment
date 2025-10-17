using System.Text.Json;
using InventorySys.Application.Common.Interfaces;
using InventorySys.Domain.Common;
using InventorySys.Infrastructure.AuditTrail;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace InventorySys.Infrastructure.Data.Interceptors;


public class FullAuditTrailInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUser _user;
    private readonly TimeProvider _dateTime;

    public FullAuditTrailInterceptor(ICurrentUser user, TimeProvider dateTime)
    {
        _user = user;
        _dateTime = dateTime;
    }
   
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        TrackChanges(eventData.Context!);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        TrackChanges(eventData.Context!);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void TrackChanges(DbContext context)
    {
        if (context == null) return;

        var entries = context.ChangeTracker.Entries<IFullAuditTrailEntity>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)
            .ToList();

        var userId = _user.Id ?? "System";
        var auditDateUtc = _dateTime.GetUtcNow();
        
        foreach (var entry in entries)
        {
            var auditEntry = CreateTrailEntry(userId,auditDateUtc, entry);
            context.Set<AuditTrailEntry>().Add(auditEntry);
        }
    }

    private static AuditTrailEntry CreateTrailEntry(string userId, DateTimeOffset auditDateUtc, EntityEntry<IFullAuditTrailEntity> entry)
    {
        var trailEntry = new AuditTrailEntry
        {
            Id = Guid.NewGuid(),
            EntityName = entry.Entity.GetType().Name,
            UserId = userId,
            DateUtc = auditDateUtc
        };

        SetAuditTrailPropertyValues(entry, trailEntry);
        SetAuditTrailNavigationValues(entry, trailEntry);
        SetAuditTrailReferenceValues(entry, trailEntry);

        return trailEntry;
    }


    private static void SetAuditTrailPropertyValues(EntityEntry entry, AuditTrailEntry trailEntry)
    {
        // Skip temp fields (that will be assigned automatically by ef core engine, for example: when inserting an entity
        foreach (var property in entry.Properties.Where(x => !x.IsTemporary))
        {
            if (property.Metadata.IsPrimaryKey())
            {
                trailEntry.PrimaryKey = property.CurrentValue?.ToString();
                continue;
            }

            SetAuditTrailPropertyValue(entry, trailEntry, property);
        }
    }

    private static void SetAuditTrailPropertyValue(EntityEntry entry, AuditTrailEntry trailEntry, PropertyEntry property)
    {
        var propertyName = property.Metadata.Name;

        switch (entry.State)
        {
            case EntityState.Added:
                trailEntry.TrailType = TrailType.Create;
                trailEntry.NewValues[propertyName] = property.CurrentValue;

                break;

            case EntityState.Deleted:
                trailEntry.TrailType = TrailType.Delete;
                trailEntry.OldValues[propertyName] = property.OriginalValue;

                break;

            case EntityState.Modified:
                if (property.IsModified && (property.OriginalValue is null || !property.OriginalValue.Equals(property.CurrentValue)))
                {
                    trailEntry.ChangedColumns.Add(propertyName);
                    trailEntry.TrailType = TrailType.Update;
                    trailEntry.OldValues[propertyName] = property.OriginalValue;
                    trailEntry.NewValues[propertyName] = property.CurrentValue;
                }

                break;
        }

        if (trailEntry.ChangedColumns.Count > 0)
        {
            trailEntry.TrailType = TrailType.Update;
        }
    }

    private static void SetAuditTrailReferenceValues(EntityEntry entry, AuditTrailEntry trailEntry)
    {
        foreach (var reference in entry.References.Where(x => x.IsModified))
        {
            var referenceName = reference.EntityEntry.Entity.GetType().Name;
            trailEntry.ChangedColumns.Add(referenceName);
        }
    }

    private static void SetAuditTrailNavigationValues(EntityEntry entry, AuditTrailEntry trailEntry)
    {
        foreach (var navigation in entry.Navigations.Where(x => x.Metadata.IsCollection && x.IsModified))
        {
            if (navigation.CurrentValue is not IEnumerable<object> enumerable)
            {
                continue;
            }

            var collection = enumerable.ToList();
            if (collection.Count == 0)
            {
                continue;
            }

            var navigationName = collection.First().GetType().Name;
            trailEntry.ChangedColumns.Add(navigationName);
        }
    }


}
