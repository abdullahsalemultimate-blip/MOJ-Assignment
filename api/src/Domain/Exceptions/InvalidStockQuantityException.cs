namespace InventorySys.Domain.Exceptions;

public class InvalidItemQuantityException : Exception
{
    public InvalidItemQuantityException(string msg)
        : base(msg)
    {
    }
}
