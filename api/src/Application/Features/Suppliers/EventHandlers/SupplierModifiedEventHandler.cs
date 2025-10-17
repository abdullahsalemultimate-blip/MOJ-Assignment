using InventorySys.Application.Common.Interfaces;
using InventorySys.Application.Common.Models;
using InventorySys.Domain.Events;

namespace InventorySys.Application.Features.Suppliers.EventHandlers;

public class SupplierModifiedEventHandler: INotificationHandler<SupplierModifiedEvent>
{
    private readonly ICacheService _cacheService;

    public SupplierModifiedEventHandler(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public Task Handle(SupplierModifiedEvent notification, CancellationToken cancellationToken)
    {
        _cacheService.Remove(CacheKey.SuppliersLookup);
        return Task.CompletedTask;
    }
}
