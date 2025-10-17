using InventorySys.Application.Features.Statistics.Dtos;

namespace InventorySys.Application.Features.Statistics;

public interface IStatisticsRepository
{
    Task<IEnumerable<ReorderProductDto>> GetProductsNeedingReorderAsync(CancellationToken cancellationToken);
    
    Task<LargestSupplierDto?> GetLargestSupplierAsync(CancellationToken cancellationToken);
    
    Task<MinimumOrderProductDto?> GetProductWithMinimumOrdersAsync(CancellationToken cancellationToken);
}