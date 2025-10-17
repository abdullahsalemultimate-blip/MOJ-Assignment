using Ardalis.GuardClauses;

namespace InventorySys.Domain.Entities;

public class Supplier: BaseAuditableEntity, IAggregateRoot, ISoftDeletable
{
    public string Name { get; private set; }
    public bool IsDeleted { get; set; }


#pragma warning disable CS8618 // Required By EfCore DBContxt
    private Supplier() { }

    public Supplier(string name)
    {
        Name = name;
    }

    public static Supplier Create(string name)
    {
        Guard.Against.NullOrWhiteSpace(name, nameof(name));
        return new(name);
    }

    public void Update(string name)
    {
        Guard.Against.NullOrWhiteSpace(name, nameof(name));
        Name = name;
    }
}
