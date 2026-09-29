using RepositoryPatternWithUOW.Core.Models;

namespace RepositoryPatternWithUOW.Core.Repository;

public interface IUnitOfWork : IDisposable
{
    public IBaseRepository<Employee> Employees { get; }
    //public IBaseRepository<Department> Departments { get; }
    public IDepartmentRepository Departments { get; }
    public Task<int> Complete();
}
