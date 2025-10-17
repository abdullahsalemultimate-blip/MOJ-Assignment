namespace InventorySys.Domain.Exceptions;

public sealed class InsufficientStockException : Exception
{
    public InsufficientStockException(int requested, int available)
        : base($"Insufficient stock. Requested: {requested}, Available: {available}.")
    {
    
    }
}
