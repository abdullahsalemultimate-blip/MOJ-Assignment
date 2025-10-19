using InventorySys.Application.Common.Models;
using InventorySys.Application.Features.Products;
using InventorySys.Application.Features.Products.Dtos;
using InventorySys.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventorySys.Infrastructure.Data.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<PaginatedList<ProductDto>> GetPagedProductsAsync(
        string? searchTerm,
        int? supplierId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = from p in _context.Products
                    join s in _context.Suppliers.IgnoreQueryFilters()
                    on p.SupplierId equals s.Id
                    select new { Product = p, Supplier = s };

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.Trim();
            query = query.Where(x => x.Product.Name.Contains(searchTerm));
        }

        if (supplierId.HasValue && supplierId.Value > 0)
            query = query.Where(x => x.Product.SupplierId == supplierId.Value);

        return await query
            .OrderBy(x => x.Product.Name)
            .Select(x => new ProductDto
            {
                Id = x.Product.Id,
                Name = x.Product.Name,
                SupplierName = x.Supplier.Name,
                UnitPrice = x.Product.UnitPrice,
                UnitsInStock = x.Product.UnitsInStock,
                UnitsOnOrder = x.Product.UnitsOnOrder
            })
            .PaginatedListAsync(pageNumber, pageSize, cancellationToken);
    }
}
