namespace InventorySys.Domain.Exceptions;

public sealed class InvalidItemQuantityException : Exception
{
    public InvalidItemQuantityException(string msg)
        : base(msg)
    {
    }
}
