using RepositoryPatternWithUOW.Core.Models;

namespace RepositoryPatternWithUOW.Core.Repository;

public interface IUnitOfWork : IDisposable
{
    public IBaseRepository<Employee, int> Employees { get; }
    //public IBaseRepository<Department> Departments { get; }
    public IDepartmentRepository<Department, int> Departments { get; }
    public Task<int> Complete();
}
