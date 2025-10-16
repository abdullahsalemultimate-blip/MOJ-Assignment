namespace InventorySys.Domain.Common;

public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
}
