using InventorySys.Domain.Entities;

namespace InventorySys.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    public DbSet<Supplier> Suppliers { get; }

    public DbSet<Product> Products { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
