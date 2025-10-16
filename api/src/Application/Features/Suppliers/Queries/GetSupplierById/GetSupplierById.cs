using InventorySys.Application.Common.Interfaces;
using InventorySys.Application.Features.Suppliers.Dtos;
using InventorySys.Domain.Entities;

namespace InventorySys.Application.Features.Suppliers.Queries.GetSupplierById;

public record GetSupplierByIdQuery(int Id) : IRequest<SupplierDto>;

public class GetSupplierByIdQueryValidator : AbstractValidator<GetSupplierByIdQuery>
{
    public GetSupplierByIdQueryValidator()
    {
    }
}

public class GetSupplierByIdQueryHandler : IRequestHandler<GetSupplierByIdQuery, SupplierDto>
{
    private readonly IRepository<Supplier> _repository;
    private readonly IMapper _mapper;

    public GetSupplierByIdQueryHandler(IRepository<Supplier> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<SupplierDto> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
    {
        var supplierDto = await _repository
            .FirstOrDefaultAsync<SupplierDto>(
                x => x.Id == request.Id,
                _mapper.ConfigurationProvider,
                cancellationToken);

        Guard.Against.NotFound(request.Id, supplierDto);
        return supplierDto;
    }
}
