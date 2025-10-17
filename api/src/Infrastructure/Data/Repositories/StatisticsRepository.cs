using System.Data;
using Dapper;
using InventorySys.Application.Features.Statistics;
using InventorySys.Application.Features.Statistics.Dtos;

namespace InventorySys.Infrastructure.Data.Repositories;

// Just demonstrating the use of Dapper alongside DbContext.
public class StatisticsRepository : IStatisticsRepository
{
    private readonly IDapperDbContext _dapperDbContext;

    public StatisticsRepository(IDapperDbContext dapperDbContext)
    {
        _dapperDbContext = dapperDbContext;
    }

    public async Task<IEnumerable<ReorderProductDto>> GetProductsNeedingReorderAsync(CancellationToken cancellationToken)
    {
        // Same Query using EfCore linq to show the both ways
        /*
        from p in _context.Products join s in _context.Suppliers.IgnoreQueryFilters()
            on p.SupplierId equals s.Id
            where p.UnitsInStock.Value <= p.ReorderLevel.Value
            orderby p.UnitsInStock ascending
            select p
        */

        const string sql = $@"
            select 
                p.Id, 
                p.Name, 
                p.UnitsInStock, 
                p.ReorderLevel, 
                s.Name AS {nameof(ReorderProductDto.SupplierName)}
            from Products p
            join Suppliers s ON p.SupplierId = s.Id
            WHERE p.UnitsInStock <= p.ReorderLevel
            order by p.UnitsInStock ASC";

        using IDbConnection connection = _dapperDbContext.GetConnection();
        var result = await connection.QueryAsync<ReorderProductDto>(new CommandDefinition(sql, cancellationToken: cancellationToken));
        return result.ToList();
    }

    public async Task<LargestSupplierDto?> GetLargestSupplierAsync(CancellationToken cancellationToken)
    {
        // Same Query using EfCore linq to show the both ways

        // var largestSupplier = await (
        //     from p in _context.Products
        //     join s in _context.Suppliers on p.SupplierId equals s.Id
        //     group p by new { p.SupplierId, s.Name } into g
        //     orderby g.Count() descending
        //     select new LargestSupplierDto
        //     {
        //         SupplierId = g.Key.SupplierId,
        //         SupplierName = g.Key.Name,
        //         TotalProducts = g.Count()
        //     }
        // ).FirstOrDefaultAsync(cancellationToken);

        const string sql = $@"
            select TOP 1
                p.SupplierId,
                s.Name AS {nameof(LargestSupplierDto.SupplierName)},
                COUNT(p.Id) AS {nameof(LargestSupplierDto.TotalProducts)}
            from Products p
            join Suppliers s ON p.SupplierId = s.Id
            group by p.SupplierId, s.Name
            order by COUNT(p.Id) desc";

        using IDbConnection connection = _dapperDbContext.GetConnection();
        return await connection.QueryFirstOrDefaultAsync<LargestSupplierDto>(new CommandDefinition(sql, cancellationToken: cancellationToken));
    }

    public async Task<MinimumOrderProductDto?> GetProductWithMinimumOrdersAsync(CancellationToken cancellationToken)
    {
        // Same Query using EfCore linq to show the both ways
        /*
        from p in _context.Products join s in _context.Suppliers.IgnoreQueryFilters()
            on p.SupplierId equals s.Id
            orderby p.UnitsOnOrder ascending
            select p
        */

        const string sql = $@"
            select TOP 1
                p.Id,
                p.Name,
                p.UnitsOnOrder,
                s.Name AS {nameof(MinimumOrderProductDto.SupplierName)}
            from Products p
            join Suppliers s ON p.SupplierId = s.Id
            order by p.UnitsOnOrder asc";

        using IDbConnection connection = _dapperDbContext.GetConnection();
        return await connection.QueryFirstOrDefaultAsync<MinimumOrderProductDto>(new CommandDefinition(sql, cancellationToken: cancellationToken));
    }
}