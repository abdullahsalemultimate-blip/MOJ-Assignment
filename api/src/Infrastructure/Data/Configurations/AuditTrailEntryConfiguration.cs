using System.Text.Json;
using InventorySys.Infrastructure.AuditTrail;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace InventorySys.Infrastructure.Data.Configurations;

public class AuditTrailEntryConfiguration : IEntityTypeConfiguration<AuditTrailEntry>
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        ReferenceHandler = null,
        WriteIndented = false,
    };
    
    public void Configure(EntityTypeBuilder<AuditTrailEntry> builder)
    {
        builder.ToTable("audit_trails");
        builder.HasKey(e => e.Id);

        builder.HasIndex(e => e.EntityName);
        builder.HasIndex(e => new { e.EntityName, e.PrimaryKey });

        builder.Property(e => e.EntityName).HasMaxLength(100).IsRequired();
        builder.Property(e => e.DateUtc).IsRequired();
        builder.Property(e => e.PrimaryKey).HasMaxLength(100);

        builder.Property(e => e.TrailType).HasConversion<string>().HasMaxLength(20);

        builder.Property(e => e.OldValues)
            .HasConversion(
                new ValueConverter<Dictionary<string, object?>, string>(
                    v => JsonSerializer.Serialize(v, _jsonOptions),
                    v => JsonSerializer.Deserialize<Dictionary<string, object?>>(v, _jsonOptions) ?? new Dictionary<string, object?>()
                )
            )
            .HasColumnType("nvarchar(max)"); // Ensure the column type is large enough

        builder.Property(e => e.NewValues)
            .HasConversion(
                new ValueConverter<Dictionary<string, object?>, string>(
                    v => JsonSerializer.Serialize(v, _jsonOptions),
                    v => JsonSerializer.Deserialize<Dictionary<string, object?>>(v, _jsonOptions) ?? new Dictionary<string, object?>()
                )
            )
            .HasColumnType("nvarchar(max)");

        
        builder.Property(e => e.ChangedColumns)
            .HasConversion(
                new ValueConverter<List<string>, string>(
                    v => JsonSerializer.Serialize(v, _jsonOptions),
                    v => JsonSerializer.Deserialize<List<string>>(v, _jsonOptions) ?? new List<string>()
                )
            )
            .HasColumnType("nvarchar(max)");
    }
}
