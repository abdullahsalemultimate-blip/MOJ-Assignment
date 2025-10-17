using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using EntityFramework.Exceptions.Common;
using InventorySys.Application.Common.Interfaces;
using InventorySys.Application.Common.Models;
using InventorySys.Domain.Common;
using InventorySys.Domain.Exceptions;
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
        return await MappingExtensions.CreatePaginatedListAsync(query, pageNumber, pageSize, cancellationToken);
    }

    public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        => await _dbSet.AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken);

    public virtual async Task<TDestination?> FirstOrDefaultAsync<TDestination>(Expression<Func<T, bool>> predicate,IConfigurationProvider configuration, CancellationToken cancellationToken = default)
        => await _dbSet.AsNoTracking().Where(predicate).ProjectTo<TDestination>(configuration).FirstOrDefaultAsync(cancellationToken);


    public virtual async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        => await _dbSet.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

    public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        => _dbSet.AnyAsync(predicate, cancellationToken);

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
    {
        try
        {
           return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (UniqueConstraintException ex)
        {
            var property = ex.ConstraintProperties?.FirstOrDefault() ?? "unknown field";
            var table = ex.SchemaQualifiedTableName ?? typeof(T).Name;
            var constraint = ex.ConstraintName ?? "unique constraint";
            var duplicatValue = ex.ConstraintProperties?[0] ?? string.Empty;

            // This is full Desceibtive message  Just for Assignement Scope
            var message =
                $"A duplicate value was found for {property} in table '{table}' (violated {constraint}). Duplicate value for {duplicatValue} ";

            throw new BusinessRuleValidationException(message, ex);
        }
        catch (ReferenceConstraintException ex)
        {
            var table = ex.SchemaQualifiedTableName ?? typeof(T).Name;
            var constraint = ex.ConstraintName ?? "foreign key constraint";

            // This is full Descriptive message  Just for Assignement Scope
            var message =
                $"Invalid reference detected while saving '{table}'. " +
                $"This usually means the related entity does not exist (violated {constraint}).";

            throw new BusinessRuleValidationException(message, ex);
        }

    }
}

public static class MappingExtensions
{
    public static Task<PaginatedList<TDestination>> PaginatedListAsync<TDestination>(this IQueryable<TDestination> queryable, int pageNumber, int pageSize, CancellationToken cancellationToken = default) where TDestination : class
        => CreatePaginatedListAsync(queryable.AsNoTracking(), pageNumber, pageSize, cancellationToken);

    public static Task<List<TDestination>> ProjectToListAsync<TDestination>(this IQueryable queryable, IConfigurationProvider configuration, CancellationToken cancellationToken = default) where TDestination : class
        => queryable.ProjectTo<TDestination>(configuration).AsNoTracking().ToListAsync(cancellationToken);

    public static async Task<PaginatedList<T>> CreatePaginatedListAsync<T>(IQueryable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var count = await source.CountAsync(cancellationToken);
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }
}
