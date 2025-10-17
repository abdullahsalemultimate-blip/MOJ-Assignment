using InventorySys.Infrastructure.AuditTrail;

namespace InventorySys.Application.Features.AuditTrailEntries;

public interface IAuditTrailRepository
{
    Task<List<AuditTrailEntry>> GetHistoryByEntityIdAsync(
        string entityName, 
        string entityId, 
        CancellationToken cancellationToken = default);
}