
using InventorySys.Application.Features.Statistics.Dtos;

namespace InventorySys.Application.Features.Statistics.Queries.GetProductWithMinimumOrdersQuery;

public record GetProductWithMinimumOrdersQuery : IRequest<MinimumOrderProductDto?>;

public class GetProductWithMinimumOrdersQueryHandler : IRequestHandler<GetProductWithMinimumOrdersQuery, MinimumOrderProductDto?>
{
    private readonly IStatisticsRepository _repository;

    public GetProductWithMinimumOrdersQueryHandler(IStatisticsRepository repository)
    {
        _repository = repository;
    }

    public Task<MinimumOrderProductDto?> Handle(GetProductWithMinimumOrdersQuery request, CancellationToken cancellationToken)
    {
        return _repository.GetProductWithMinimumOrdersAsync(cancellationToken);
    }
}