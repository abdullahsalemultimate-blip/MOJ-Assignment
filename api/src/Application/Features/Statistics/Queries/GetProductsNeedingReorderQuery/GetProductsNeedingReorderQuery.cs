

using InventorySys.Application.Features.Statistics.Dtos;

namespace InventorySys.Application.Features.Statistics.Queries.GetProductsNeedingReorderQuery;

public record GetProductsNeedingReorderQuery : IRequest<IEnumerable<ReorderProductDto>>;

public class GetProductsNeedingReorderQueryHandler : IRequestHandler<GetProductsNeedingReorderQuery, IEnumerable<ReorderProductDto>>
{
    private readonly IStatisticsRepository _repository;

    public GetProductsNeedingReorderQueryHandler(IStatisticsRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<ReorderProductDto>> Handle(GetProductsNeedingReorderQuery request, CancellationToken cancellationToken)
    {
        return _repository.GetProductsNeedingReorderAsync(cancellationToken);
    }
}

