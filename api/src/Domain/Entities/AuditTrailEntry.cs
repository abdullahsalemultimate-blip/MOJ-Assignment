namespace InventorySys.Infrastructure.AuditTrail;

public class AuditTrailEntry
{
    public required Guid Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public TrailType TrailType { get; set; }

    public DateTimeOffset DateUtc { get; set; }

    public required string EntityName { get; set; }

    public string? PrimaryKey { get; set; }

    public Dictionary<string, object?> OldValues { get; set; } = [];

    public Dictionary<string, object?> NewValues { get; set; } = [];

    public List<string> ChangedColumns { get; set; } = [];
}
