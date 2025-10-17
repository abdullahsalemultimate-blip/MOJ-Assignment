using InventorySys.Application.Common.Interfaces;
using InventorySys.Application.Common.Models;
using InventorySys.Domain.Entities;

namespace InventorySys.Application.Features.Suppliers.Queries.GetSuppliersLookup;

public record GetSuppliersLookupQuery : IRequest<List<LookupDto>>
{
}


public class GetSuppliersLookupQueryHandler : IRequestHandler<GetSuppliersLookupQuery, List<LookupDto>>
{
    private readonly IRepository<Supplier> _repository;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;

    public GetSuppliersLookupQueryHandler(IRepository<Supplier> repository, IMapper mapper, ICacheService cacheService)
    {
        _repository = repository;
        _mapper = mapper;
        _cacheService = cacheService;
    }


    public async Task<List<LookupDto>> Handle(GetSuppliersLookupQuery request, CancellationToken cancellationToken)
    {
        
        return await _cacheService.GetOrCreateAsync(CacheKey.SuppliersLookup, async () =>
        {
            return await _repository.GetAllAsync<LookupDto>(_mapper.ConfigurationProvider, cancellationToken);
        });

    }
}
