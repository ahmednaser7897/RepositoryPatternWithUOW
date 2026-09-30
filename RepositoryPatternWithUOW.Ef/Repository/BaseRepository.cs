using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RepositoryPatternWithUOW.Core.Repository;
using RepositoryPatternWithUOW.Core.Specification;

namespace RepositoryPatternWithUOW.Ef.Repository;

public class BaseRepository<TEntity, TKey>(AppDbContext context)
: IBaseRepository<TEntity, TKey>
where TEntity : class, IEntity<TKey>
{
    protected readonly AppDbContext _context = context;

    public async Task<TEntity?> GetById(TKey id)
    {
        //Set<T>() return the DbSet for the specified entity type.
        return await _context.Set<TEntity>().FindAsync(id);
    }

    public async Task<List<TEntity>> GetAll()
    {
        return await _context.Set<TEntity>().ToListAsync();
    }

    public async Task<bool> Add(TEntity model)
    {
        await _context.Set<TEntity>().AddAsync(model);
        return await SaveChangesAsync();
    }

    public async Task<bool> Update(TEntity model)
    {
        _context.Set<TEntity>().Update(model);
        return await SaveChangesAsync();
    }

    public async Task<bool> Remove(TKey id)
    {
        var model = await GetById(id);
        if (model != null)
        {
            _context.Set<TEntity>().Remove(model);
            return await SaveChangesAsync();
        }
        return false;
    }

    public async Task<bool> SaveChangesAsync()
    {
        var result = await _context.SaveChangesAsync() > 0;
        return result;
    }

    public async Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate, List<string>? includes = null)
    {
        IQueryable<TEntity> query = _context.Set<TEntity>();

        if (includes != null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }
        return await query.FirstOrDefaultAsync(predicate);
    }

    public async Task<List<TEntity>> FindAllAsync(
        Expression<Func<TEntity, bool>> predicate,
        List<string>? includes = null,
        int? take = null,
        int? skip = null,
         Expression<Func<TEntity, object>>? orderBy = null,
         bool isAscending = true)
    {
        IQueryable<TEntity> query = _context.Set<TEntity>().Where(predicate);
        if (includes != null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }
        if (skip != null && skip > 0) query = query.Skip(skip.Value);
        if (take != null && take > 0) query = query.Take(take.Value);

        if (orderBy != null)
        {
            query = isAscending ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
        }
        return await query.ToListAsync();
    }

    public async Task<TEntity?> GetByIdWithSpec(ISpecification<TEntity, TKey> spec)
    {
        var BaseQurey = _context.Set<TEntity>().AsNoTracking();
        var query = SpecificationEvaluator.GenerateQuery<TEntity, TKey>(
           BaseQurey,
           spec
        );
        return await query.FirstOrDefaultAsync();
    }

    public async Task<List<TEntity>> GetAllWithSpec(ISpecification<TEntity, TKey> spec)
    {
        var BaseQurey = _context.Set<TEntity>().AsNoTracking();
        var query = SpecificationEvaluator.GenerateQuery<TEntity, TKey>(
           BaseQurey,
           spec
        );
        return await query.ToListAsync();
    }
}

