using System.Linq.Expressions;

namespace RepositoryPatternWithUOW.Core.Specification;

public interface ISpecification<TEntity, TKey> where TEntity : IEntity<TKey>
{

    //1- where condition
    Expression<Func<TEntity, bool>>? Criteria { get; }

    //2- Join ralated entities
    List<Expression<Func<TEntity, object>>> Includes { get; }

    //3-order by with flag descending
    List<OrderExpression<TEntity>> OrderBy { get; }

    //4- Pagination
    int? Take { get; }//how many records to take
    int? Skip { get; }//how many records to skip
    bool IsPaginationEnabled { get; }
}
