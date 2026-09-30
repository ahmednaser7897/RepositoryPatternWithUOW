using RepositoryPatternWithUOW.Core.Models;
using RepositoryPatternWithUOW.Core.Specification;

namespace RepositoryPatternWithUOW.Core.Repository;

public interface IDepartmentRepository<Department, TKey> :
IBaseRepository<Department, TKey>
where Department : class, IEntity<TKey>
{
    public Task<List<Employee>> GetEmployeesOfDepartment(int id);
}
