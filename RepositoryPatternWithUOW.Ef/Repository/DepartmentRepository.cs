using Microsoft.EntityFrameworkCore;
using RepositoryPatternWithUOW.Core.Models;
using RepositoryPatternWithUOW.Core.Repository;
using RepositoryPatternWithUOW.Core.Specification;

namespace RepositoryPatternWithUOW.Ef.Repository;

public class DepartmentRepository<Department, TKey>(AppDbContext context)
: BaseRepository<Department, TKey>(context),
IDepartmentRepository<Department, TKey>
where Department : class, IEntity<TKey>
{
    public async Task<List<Employee>> GetEmployeesOfDepartment(int id)
    {
        var res = await _context.Departments.
        Include(e => e.Employees).
        FirstOrDefaultAsync(e => e.Id == id);
        return res?.Employees ?? [];
    }
}
