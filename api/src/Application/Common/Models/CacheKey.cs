namespace InventorySys.Application.Common.Models;

public sealed record CacheKey(string Value)
{
    public static CacheKey SuppliersLookup => new("SuppliersLookup");
    
    public override string ToString() => Value;
}
