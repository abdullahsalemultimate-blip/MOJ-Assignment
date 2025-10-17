using InventorySys.Application.Features.Products;
using InventorySys.Application.Features.Products.Dtos;

namespace InventorySys.Application.Products.Queries.GetProducts;

public record GetProductsQuery : PaginatedRequest<ProductDto>
{
    public string? SearchTerm { get; set; }
    public int? SupplierId { get; set; }
}


public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, PaginatedList<ProductDto>>
{
     private readonly IProductRepository _repository;

    public GetProductsQueryHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<PaginatedList<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetPagedProductsAsync(
            request.SearchTerm,
            request.SupplierId,
            request.PageNumber,
            request.PageSize,
            cancellationToken);
    }
}
