using System.Linq.Expressions;

namespace RepositoryPatternWithUOW.Core.Specification;

public class OrderExpression<TEntity>
{
    //Expression of orderby and isdescending
    //OrderBy( e => e.Id) =>OrderExpression= e=> e.Id, IsDescending= false
    //OrderByDescending(e => e.Id) =>OrderExpression= e=> e.Id, IsDescending= true
    public required Expression<Func<TEntity, object>> OrderBy { get; set; }
    public bool IsDescending { get; set; } = false;
}
