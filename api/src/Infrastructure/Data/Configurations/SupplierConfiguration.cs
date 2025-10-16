using InventorySys.Domain.Constants;
using InventorySys.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventorySys.Infrastructure.Data.Configurations;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.Property(s=> s.Name)
            .HasMaxLength(DomainConsts.NameDefaultMaxLength)
            .IsRequired();
    }
}
