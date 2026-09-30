using Microsoft.EntityFrameworkCore;

namespace RepositoryPatternWithUOW.Core.Specification;

//select * from TEntity where Criteria  OrderBy Desc Take Skip  includes=left join related

public static class SpecificationEvaluator
{
    public static IQueryable<TEntity> GenerateQuery<TEntity, TKey>(
        IQueryable<TEntity> inputQueryable,
        ISpecification<TEntity, TKey> specification
    )
    where TEntity : class, IEntity<TKey>
    {
        var queryable = inputQueryable;

        //apply criteria
        if (specification.Criteria != null)
        {
            //Select * from Product where Name="productName"
            queryable = queryable.Where(specification.Criteria);
        }
        //apply includes
        if (specification.Includes?.Count > 0)
        {
            //select * from product p join productcategory pc on p.productcategoryid = pc.id
            foreach (var include in specification.Includes)
            {
                queryable = queryable.Include(include);
            }
        }
        //apply order by
        if (specification.OrderBy.Count > 0)
        {
            //1- get the first order by expression
            OrderExpression<TEntity> firstOrder = specification.OrderBy[0];
            //2- check id the order is descending or ascending and then order by
            IOrderedQueryable<TEntity> orderedQueryable =
             firstOrder.IsDescending ? queryable.OrderByDescending(firstOrder.OrderBy)
             : queryable.OrderBy(firstOrder.OrderBy);
            //2- get the rest of the order by expressions
            var remainingOrders = specification.OrderBy.Skip(1);
            foreach (var order in remainingOrders)
            {
                orderedQueryable = order.IsDescending ?
                orderedQueryable.OrderByDescending(order.OrderBy)
                : orderedQueryable.OrderBy(order.OrderBy);
            }
            queryable = orderedQueryable;
        }
        //apply pagination
        if (specification.IsPaginationEnabled)
        {
            queryable = queryable.Skip(specification.Skip ?? 0).Take(specification.Take ?? 0);
        }
        return queryable;
    }
}


