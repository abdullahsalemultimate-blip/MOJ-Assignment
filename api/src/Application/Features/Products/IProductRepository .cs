using InventorySys.Application.Common.Interfaces;
using InventorySys.Application.Common.Models;
using InventorySys.Application.Features.Products.Dtos;
using InventorySys.Domain.Entities;

namespace InventorySys.Application.Features.Products;
public interface IProductRepository : IRepository<Product>
{
    Task<PaginatedList<ProductDto>> GetPagedProductsAsync(
        string? searchTerm,
        int? supplierId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);
}