namespace InventorySys.Application.Features.AuditTrailEntries.Queries.GetEntityHistory;

public class AuditEntryDto
{
    public Guid TrailId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTimeOffset DateUtc { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public string? PrimaryKey { get; set; }
    
    /// <summary>
    /// A list of specific changes (property name, old value, new value).
    /// Used primarily for the 'Update' action.
    /// </summary>
    public List<ChangeDetailDto> Changes { get; set; } = new();

    /// <summary>
    /// Used for cases like 'Create' or 'Delete' where a full snapshot is needed.
    /// This will contain a JSON string of the object state.
    /// </summary>
    public string FullSnapshot { get; set; } = string.Empty;
}

public class ChangeDetailDto
{
    public string Property { get; set; } = string.Empty;
    
    public string? OldValue { get; set; }
    
    public string? NewValue { get; set; }
}