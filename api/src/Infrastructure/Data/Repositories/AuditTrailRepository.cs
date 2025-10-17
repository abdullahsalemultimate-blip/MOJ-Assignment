using InventorySys.Application.Features.AuditTrailEntries;
using InventorySys.Infrastructure.AuditTrail;
using Microsoft.EntityFrameworkCore;

namespace InventorySys.Infrastructure.Data.Repositories;

public class AuditTrailRepository : IAuditTrailRepository
{
    private readonly ApplicationDbContext _context;

    public AuditTrailRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<AuditTrailEntry>> GetHistoryByEntityIdAsync(
        string entityName, 
        string entityId, 
        CancellationToken cancellationToken = default)
    {
        return await _context.AuditTrailEntries
            .Where(a => a.EntityName == entityName && a.PrimaryKey == entityId)
            .OrderByDescending(a => a.DateUtc)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}