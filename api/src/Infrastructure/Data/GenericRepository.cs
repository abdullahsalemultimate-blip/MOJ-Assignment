using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using InventorySys.Application.Common.Interfaces;
using InventorySys.Application.Common.Models;
using InventorySys.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace InventorySys.Infrastructure.Data;

public class GenericRepository<T> : IRepository<T> where T : class, IAggregateRoot
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _dbSet.FindAsync([id], cancellationToken);

    public virtual async Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _dbSet.AsNoTracking().ToListAsync(cancellationToken);


    public virtual async Task<List<TDestination>> GetAllAsync<TDestination>(
        IConfigurationProvider configuration,
        CancellationToken cancellationToken = default)
        where TDestination : class
    {
        return await _dbSet.AsNoTracking().ProjectTo<TDestination>(configuration).ToListAsync(cancellationToken);
        
    }

    public virtual async Task<PaginatedList<T>> GetAllPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        => await _dbSet.AsNoTracking().PaginatedListAsync(pageNumber, pageSize, cancellationToken);

    public virtual async Task<PaginatedList<TDestination>> GetAllPagedAsync<TDestination>(
        IConfigurationProvider configuration,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
        where TDestination : class
    {
        var query = _dbSet.AsNoTracking().ProjectTo<TDestination>(configuration);
        return await PaginatedList<TDestination>.CreateAsync(query, pageNumber, pageSize, cancellationToken);
    }

    public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        => await _dbSet.AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken);

    public virtual async Task<TDestination?> FirstOrDefaultAsync<TDestination>(Expression<Func<T, bool>> predicate,IConfigurationProvider configuration, CancellationToken cancellationToken = default)
        => await _dbSet.AsNoTracking().Where(predicate).ProjectTo<TDestination>(configuration).FirstOrDefaultAsync(cancellationToken);


    public virtual async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        => await _dbSet.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

    public virtual void Add(T entity)
        => _dbSet.Add(entity);

    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        => await _dbSet.AddAsync(entity, cancellationToken);

    public virtual async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        => await _dbSet.AddRangeAsync(entities, cancellationToken);

    public virtual void Update(T entity)
        => _dbSet.Update(entity);

    public virtual void Remove(T entity)
        => _dbSet.Remove(entity);

    public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}

public static class MappingExtensions
{
    public static Task<PaginatedList<TDestination>> PaginatedListAsync<TDestination>(this IQueryable<TDestination> queryable, int pageNumber, int pageSize, CancellationToken cancellationToken = default) where TDestination : class
        => PaginatedList<TDestination>.CreateAsync(queryable.AsNoTracking(), pageNumber, pageSize, cancellationToken);

    public static Task<List<TDestination>> ProjectToListAsync<TDestination>(this IQueryable queryable, IConfigurationProvider configuration, CancellationToken cancellationToken = default) where TDestination : class
        => queryable.ProjectTo<TDestination>(configuration).AsNoTracking().ToListAsync(cancellationToken);
}