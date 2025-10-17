namespace InventorySys.Domain.ValueObjects;

public  readonly record struct ItemQuantity
{
    // Just Extra Validation
    private const int MaximumQuantity = int.MaxValue;

    public int Value { get; }

    private ItemQuantity(int value) => Value = value;
    
    public static ItemQuantity Create(int value)
    {
        if (value < 0)
            throw new InvalidItemQuantityException("Quantity cannot be negative");

        if (value > MaximumQuantity)
            throw new InvalidItemQuantityException($"Quantity cannot exceed the maximum limit ({MaximumQuantity})");
            
       return new (value);
    }

    public ItemQuantity Add(int amount) => Create(checked(amount + Value)); // jUST Double Cjecks to make sure there is no overflows.
    public ItemQuantity Substract(int amount) => Create(checked(Value - amount)); // jUST Double Cjecks to make sure there is no overflows.

    public static implicit operator int(ItemQuantity quantity) => quantity.Value;
    public static implicit operator ItemQuantity(int value) => Create(value);

    public override string ToString() => Value.ToString();
}