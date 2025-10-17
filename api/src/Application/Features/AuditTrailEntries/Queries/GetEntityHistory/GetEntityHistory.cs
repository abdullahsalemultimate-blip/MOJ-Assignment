using System.Text.Json;
using InventorySys.Infrastructure.AuditTrail;

namespace InventorySys.Application.Features.AuditTrailEntries.Queries.GetEntityHistory;

public record GetEntityHistoryQuery(string EntityName, string EntityId) : IRequest<List<AuditEntryDto>>;

public class GetEntityHistoryQueryHandler : IRequestHandler<GetEntityHistoryQuery, List<AuditEntryDto>>
{
    private readonly IAuditTrailRepository _repository;

    public GetEntityHistoryQueryHandler(IAuditTrailRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<AuditEntryDto>> Handle(GetEntityHistoryQuery request, CancellationToken cancellationToken)
    {

        var auditEntries = await _repository.GetHistoryByEntityIdAsync(request.EntityName, request.EntityId);

        var history = auditEntries
            .Select(MapToDto)
            .ToList();

        return history;
    }
    
    private static AuditEntryDto MapToDto(AuditTrailEntry entry)
    {
        var dto = new AuditEntryDto
        {
            TrailId = entry.Id,
            EntityName = entry.EntityName,
            Action = entry.TrailType.ToString(), 
            UserId = entry.UserId,
            DateUtc = entry.DateUtc,
            PrimaryKey = entry.PrimaryKey
        };

        if (entry.TrailType == TrailType.Create)
        {
            // For CREATE, serialize the full NewValues dictionary for a snapshot
            dto.FullSnapshot = JsonSerializer.Serialize(entry.NewValues);
        }
        else if (entry.TrailType == TrailType.Delete)
        {
            // For DELETE, serialize the full OldValues dictionary for a snapshot
            dto.FullSnapshot = JsonSerializer.Serialize(entry.OldValues);
        }

        if (entry.TrailType == TrailType.Update && entry.ChangedColumns.Count > 0)
        {
            foreach (var propertyName in entry.ChangedColumns)
            {
                entry.OldValues.TryGetValue(propertyName, out object? oldValue);
                entry.NewValues.TryGetValue(propertyName, out object? newValue);

                dto.Changes.Add(new ChangeDetailDto
                {
                    Property = propertyName,
                    OldValue = oldValue?.ToString(), 
                    NewValue = newValue?.ToString()
                });
            }
        }
        
        return dto;
    }
}


