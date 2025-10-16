using InventorySys.Application.Common.Interfaces;
using InventorySys.Application.Features.Suppliers.Dtos;
using InventorySys.Domain.Entities;

namespace InventorySys.Application.Features.Suppliers.Queries.GetAllSuppliers;

public record GetAllSuppliersQuery : IRequest<List<SupplierDto>>;

public class GetAllSuppliersQueryHandler : IRequestHandler<GetAllSuppliersQuery, List<SupplierDto>>
{
    private readonly IRepository<Supplier> _repository;
    private readonly IMapper _mapper;

    public GetAllSuppliersQueryHandler(IRepository<Supplier> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public Task<List<SupplierDto>> Handle(GetAllSuppliersQuery request, CancellationToken cancellationToken)
    {
        return _repository.GetAllAsync<SupplierDto>(_mapper.ConfigurationProvider, cancellationToken);
    }
}
