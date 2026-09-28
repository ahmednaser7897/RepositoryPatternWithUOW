using Microsoft.EntityFrameworkCore;
using RepositoryPatternWithUOW.Core.Repository;
using System.Linq.Expressions;

namespace RepositoryPatternWithUOW.Ef.Repository;

public class BaseRepository<T>(AppDbContext context) : IBaseRepository<T> where T : class
{
    protected readonly AppDbContext _context = context;

    public async Task<T?> GetById(int id)
    {
        //Set<T>() return the DbSet for the specified entity type.
        return await _context.Set<T>().FindAsync(id);
    }

    public async Task<List<T>> GetAll()
    {
        return await _context.Set<T>().ToListAsync();
    }

    public async Task<bool> Add(T model)
    {
        await _context.Set<T>().AddAsync(model);
        return await SaveChangesAsync();
    }

    public async Task<bool> Update(T model)
    {
        _context.Set<T>().Update(model);
        return await SaveChangesAsync();
    }

    public async Task<bool> Remove(int id)
    {
        var model = await GetById(id);
        if (model != null)
        {
            _context.Set<T>().Remove(model);
            return await SaveChangesAsync();
        }
        return false;
    }

    public async Task<bool> SaveChangesAsync()
    {
        var result = await _context.SaveChangesAsync() > 0;
        return result;
    }

    public async Task<T?> FindAsync(Expression<Func<T, bool>> predicate, List<string>? includes = null)
    {
        IQueryable<T> query = _context.Set<T>();

        if (includes != null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }
        return await query.FirstOrDefaultAsync(predicate);
    }

    public async Task<List<T>> FindAllAsync(
        Expression<Func<T, bool>> predicate,
        List<string>? includes = null,
        int? take = null,
        int? skip = null,
         Expression<Func<T, object>>? orderBy = null,
         bool isAscending = true)
    {
        IQueryable<T> query = _context.Set<T>().Where(predicate);
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

}

