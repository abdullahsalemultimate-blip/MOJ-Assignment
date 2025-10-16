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

    public GetSuppliersLookupQueryHandler(IRepository<Supplier> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }


    public async Task<List<LookupDto>> Handle(GetSuppliersLookupQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync<LookupDto>(_mapper.ConfigurationProvider, cancellationToken);
    }
}
