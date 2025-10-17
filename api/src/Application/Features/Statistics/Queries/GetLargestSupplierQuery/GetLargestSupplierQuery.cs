using InventorySys.Application.Features.Statistics.Dtos;

namespace InventorySys.Application.Features.Statistics.Queries.GetLargestSupplierQuery;
public record GetLargestSupplierQuery : IRequest<LargestSupplierDto?>;


public class GetLargestSupplierQueryHandler : IRequestHandler<GetLargestSupplierQuery, LargestSupplierDto?>
{
    private readonly IStatisticsRepository _repository;

    public GetLargestSupplierQueryHandler(IStatisticsRepository repository)
    {
        _repository = repository;
    }

    public Task<LargestSupplierDto?> Handle(GetLargestSupplierQuery request, CancellationToken cancellationToken)
    {
        return _repository.GetLargestSupplierAsync(cancellationToken);
    }
}

