using RepositoryPatternWithUOW.Core.Models;

namespace RepositoryPatternWithUOW.Core.Repository;

public interface IDepartmentRepository : IBaseRepository<Department>
{
    public Task<List<Employee>> GetEmployeesOfDepartment(int id);
}
