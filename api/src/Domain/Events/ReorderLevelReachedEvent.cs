namespace InventorySys.Domain.Events;

public class ReorderLevelReachedEvent : BaseEvent
{
    public ReorderLevelReachedEvent(Product product)
    {
        Product = product;
    }

    public Product Product { get; }
}
