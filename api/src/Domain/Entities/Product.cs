using Ardalis.GuardClauses;

namespace InventorySys.Domain.Entities;

public class Product : BaseAuditableEntity, IAggregateRoot, IFullAuditTrailEntity
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
    
    private Product(
        string name,
        QuantityUnit quantityPerUnit,
        int supplierId,
        decimal unitPrice,
        ItemQuantity unitsInStock,
        ItemQuantity unitsOnOrder,
        ItemQuantity reorderLevel)
    {
        Name = name;
        QuantityPerUnit = quantityPerUnit;
        SupplierId = supplierId;
        UnitPrice = unitPrice;
        UnitsInStock = unitsInStock;
        UnitsOnOrder = unitsOnOrder;
        ReorderLevel = reorderLevel;
    }

    public static Product Create(
        string name,
        QuantityUnit quantityPerUnit,
        int supplierId,
        decimal unitPrice,
        int unitsInStock = 0,
        int unitsOnOrder = 0,
        int reorderLevel = 0)
    {
        Guard.Against.NullOrWhiteSpace(name, nameof(name));
        Guard.Against.OutOfRange(unitPrice, nameof(unitPrice), 0, int.MaxValue);
        Guard.Against.NegativeOrZero(supplierId, nameof(supplierId));

        return new Product(
            name,
            quantityPerUnit,
            supplierId,
            unitPrice,
            ItemQuantity.Create(unitsInStock),
            ItemQuantity.Create(unitsOnOrder),
            ItemQuantity.Create(reorderLevel));
    }

    public void UpdateDetails(
        string name,
        QuantityUnit quantityPerUnit,
        decimal unitPrice,
        int supplierId,
        ItemQuantity unitsInStock,
        ItemQuantity unitsOnOrder,
        ItemQuantity reorderLevel)
    {
        Guard.Against.NullOrWhiteSpace(name, nameof(name));
        Guard.Against.OutOfRange(unitPrice, nameof(unitPrice), 0, int.MaxValue);
        Guard.Against.NegativeOrZero(supplierId, nameof(supplierId));

        Name = name;
        QuantityPerUnit = quantityPerUnit;
        UnitPrice = unitPrice;
        UnitsInStock = unitsInStock;
        UnitsOnOrder = unitsOnOrder;
        ReorderLevel = reorderLevel;

        if (UnitsInStock <= ReorderLevel)
        {
            AddDomainEvent(new ReorderLevelReachedEvent(this));
        }
    }

    public void IncreaseStock(int amount)
    {
        Guard.Against.OutOfRange(amount, nameof(amount), 1, int.MaxValue);
        UnitsInStock = UnitsInStock.Add(amount);
    }

    public void DecreaseStock(int amount)
    {
        Guard.Against.OutOfRange(amount, nameof(amount), 1, int.MaxValue);

        if (UnitsInStock < amount) 
            throw new InsufficientStockException(amount, UnitsInStock);

        var previousStock = UnitsInStock;

        UnitsInStock = UnitsInStock.Substract( amount);

        if (previousStock > ReorderLevel && UnitsInStock <= ReorderLevel)
        {
            AddDomainEvent(new ReorderLevelReachedEvent(this));
        }
    }

}
