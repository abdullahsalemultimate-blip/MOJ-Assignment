using AutoMapper;
using InventorySys.Application.Common.Interfaces;
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
        var query = from p in _context.Products join s in _context.Suppliers.IgnoreQueryFilters()
            on p.SupplierId equals s.Id
            where p.UnitsInStock.Value <= p.ReorderLevel.Value
            select p;


        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.Trim();
            query = query.Where(p => p.Name.Contains(searchTerm));
        }

        if (supplierId.HasValue && supplierId.Value > 0)
            query = query.Where(p => p.SupplierId == supplierId.Value);

        return await query
            .OrderBy(p => p.Name)
            .Select(x=> new ProductDto
            {
                Id = x.Id,
                Name = x.Name,
                SupplierName = x.Supplier.Name,
                UnitPrice = x.UnitPrice,
                UnitsInStock = x.UnitsInStock,
                UnitsOnOrder = x.UnitsOnOrder
            })
            .PaginatedListAsync(pageNumber, pageSize, cancellationToken);
    }
}
