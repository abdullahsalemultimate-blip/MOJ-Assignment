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
        SetName(name);
    }

    public void Update(string name)
    {
        SetName(name);
    }
    
    private void SetName(string name)
    {
        Guard.Against.NullOrEmpty(name, nameof(name));
        Name = name;
    }
}
