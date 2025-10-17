namespace InventorySys.Domain.Events;

public class SupplierModifiedEvent(Supplier supplier) : BaseEvent
{
    public Supplier Supplier { get; set; } = supplier;
}
