using System.Linq.Expressions;
using InventorySys.Application.Common.Models;

namespace InventorySys.Application.Common.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<List<TDestination>> GetAllAsync<TDestination>(
        IConfigurationProvider configuration,
        CancellationToken cancellationToken = default)
        where TDestination : class;

    Task<PaginatedList<T>> GetAllPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    Task<PaginatedList<TDestination>> GetAllPagedAsync<TDestination>(
        IConfigurationProvider configuration,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
        where TDestination : class;

    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<TDestination?> FirstOrDefaultAsync<TDestination>(Expression<Func<T, bool>> predicate, IConfigurationProvider configuration, CancellationToken cancellationToken = default);
    Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    void Add(T entity);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    void Update(T entity);
    void Remove(T entity);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
