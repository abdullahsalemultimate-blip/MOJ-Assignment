using InventorySys.Domain.Constants;
using InventorySys.Domain.Entities;
using InventorySys.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventorySys.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(p => p.ReorderLevel)
            .HasConversion(
                quantity => quantity.Value,
                qValue => ItemQuantity.Create(qValue));

        builder.Property(p => p.UnitsOnOrder)
            .HasConversion(
                quantity => quantity.Value,
                qValue => ItemQuantity.Create(qValue));
          
        builder.Property(p => p.UnitsInStock)
            .HasConversion(
                quantity => quantity.Value,
                qValue => ItemQuantity.Create(qValue));

        builder.Property(p => p.Name)
            .HasMaxLength(DomainConsts.NameDefaultMaxLength)
            .IsRequired();

        builder.HasOne<Supplier>()
        .WithMany()
        .HasForeignKey(p => p.SupplierId);
    }
}
