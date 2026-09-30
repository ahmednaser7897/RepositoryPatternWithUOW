using System.Linq.Expressions;
using RepositoryPatternWithUOW.Core.Specification;

namespace RepositoryPatternWithUOW.Core.Repository;

public interface IBaseRepository<TEntity, TKey> where TEntity : IEntity<TKey>
{
    Task<TEntity?> GetById(TKey id);
    Task<List<TEntity>> GetAll();
    Task<bool> Add(TEntity model);
    Task<bool> Update(TEntity model);
    Task<bool> Remove(TKey id);
    Task<bool> SaveChangesAsync();
    Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate, List<string>? includes = null);
    Task<List<TEntity>> FindAllAsync(
        Expression<Func<TEntity, bool>> predicate,
        List<string>? includes = null,
        int? take = null,
        int? skip = null,
        Expression<Func<TEntity, object>>? orderBy = null,
        bool isAscending = true
    );
    //---------------------------------------------
    Task<TEntity?> GetByIdWithSpec(ISpecification<TEntity, TKey> spec);
    Task<List<TEntity>> GetAllWithSpec(ISpecification<TEntity, TKey> spec);


}
