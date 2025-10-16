using InventorySys.Domain.Constants;
using InventorySys.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventorySys.Infrastructure.Data.Configurations;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.Property(s => s.Name)
            .HasMaxLength(DomainConsts.NameDefaultMaxLength)
            .IsRequired();

        builder.HasQueryFilter(r => !r.IsDeleted);

        // here i use Filtered Index (it not help within the scope of this assignment but over all this will enhance query performance, especially in tables with a significant number of soft-deleted records)
        builder
            .HasIndex(r => r.IsDeleted)
            .HasFilter("IsDeleted = 0");
    }
}
