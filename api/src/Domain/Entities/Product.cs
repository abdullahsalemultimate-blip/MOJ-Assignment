using Ardalis.GuardClauses;

namespace InventorySys.Domain.Entities;

public class Product : BaseAuditableEntity, IAggregateRoot
{
    public string Name { get; private set; }
    public QuantityUnit QuantityPerUnit { get; private set; }
    public ItemQuantity UnitsInStock { get; private set; }
    public ItemQuantity UnitsOnOrder { get; private set; }
    public ItemQuantity ReorderLevel { get; private set; }
    public decimal UnitPrice { get; private set; }
    public int SupplierId { get; private set; }


    #pragma warning disable CS8618 //  Required By EfCore DBContxt
    private Product() { }
    
    public Product(string name, ItemQuantity unitsInStock, ItemQuantity reorderLevel, int supplierId, QuantityUnit quantityPerUnit, decimal unitPrice)
    {
        Guard.Against.OutOfRange(unitPrice, nameof(unitPrice), 1, int.MaxValue);
        Guard.Against.Empty(name, nameof(name));
        Name = name;
        UnitsInStock = unitsInStock;
        ReorderLevel = reorderLevel;
        SupplierId = supplierId;
        QuantityPerUnit = quantityPerUnit;
        UnitPrice = unitPrice;
    }
}
