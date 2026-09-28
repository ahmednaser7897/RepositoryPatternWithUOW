using System.Linq.Expressions;

namespace RepositoryPatternWithUOW.Core.Repository;

public interface IBaseRepository<T> where T : class
{
    Task<T?> GetById(int id);
    Task<List<T>> GetAll();
    Task<bool> Add(T model);
    Task<bool> Update(T model);
    Task<bool> Remove(int id);
    Task<bool> SaveChangesAsync();
    Task<T?> FindAsync(Expression<Func<T, bool>> predicate, List<string>? includes = null);
    Task<List<T>> FindAllAsync(
        Expression<Func<T, bool>> predicate,
        List<string>? includes = null,
        int? take = null,
        int? skip = null,
        Expression<Func<T, object>>? orderBy = null,
        bool isAscending = true
    );


}
