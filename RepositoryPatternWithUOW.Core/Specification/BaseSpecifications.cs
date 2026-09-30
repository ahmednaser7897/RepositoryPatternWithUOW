using System.Linq.Expressions;

namespace RepositoryPatternWithUOW.Core.Specification;

public class BaseSpecifications<TEntity, TKey>
: ISpecification<TEntity, TKey>
where TEntity : class, IEntity<TKey>
{

    public BaseSpecifications()
    {
        //default spec = get all entities
        Criteria = _ => true;
    }
    //Applay where condition
    public Expression<Func<TEntity, bool>>? Criteria { get; private set; }
    public void AddCriteria(Expression<Func<TEntity, bool>> criteria)
    {
        //custom spec = user criteria
        Criteria = criteria;
    }

    public List<Expression<Func<TEntity, object>>> Includes { get; } = [];
    public void AddInclude(Expression<Func<TEntity, object>> include)
    {
        Includes.Add(include);
    }
    //Applay order by
    public List<OrderExpression<TEntity>> OrderBy { get; } = [];

    public void AddOrderBy(Expression<Func<TEntity, object>> orderBy, bool isdesc = false)
    {
        OrderBy.Add(new OrderExpression<TEntity> { OrderBy = orderBy, IsDescending = isdesc });
    }
    //Apply pagination
    public int? Take { get; private set; }
    public int? Skip { get; private set; }
    public bool IsPaginationEnabled { get; private set; }
    public void AddPagination(int PageSize, int PageNumber)// Pagesize=10,PageNumber=3 ==> Skip=20,Take=10
    {
        Skip = PageSize * (PageNumber - 1);
        Take = PageSize;
        IsPaginationEnabled = true;
    }

}


