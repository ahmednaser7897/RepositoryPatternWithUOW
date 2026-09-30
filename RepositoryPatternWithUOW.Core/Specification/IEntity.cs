namespace RepositoryPatternWithUOW.Core.Specification;

public interface IEntity<TKey>
{
    public TKey Id { get; set; }
}
