using InventorySys.Application.Features.Products;
using InventorySys.Application.Features.Products.Dtos;
using InventorySys.Domain.Enums;

namespace InventorySys.Application.Products.Queries.GetProductById;

public record GetProductByIdQuery(int Id)  : IRequest<ProductDetailVm>;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDetailVm>
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;

    public GetProductByIdQueryHandler(IProductRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ProductDetailVm> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _repository.FirstOrDefaultAsync<ProductDetailDto>(
            x => x.Id == request.Id,
            _mapper.ConfigurationProvider,
            cancellationToken);

        Guard.Against.NotFound(request.Id, product);

        return new ProductDetailVm
        {
            QuantityUnits = EnumExtensions.ToLookupList<QuantityUnit>(),
            ProductDetail = product
        };
    }
}
