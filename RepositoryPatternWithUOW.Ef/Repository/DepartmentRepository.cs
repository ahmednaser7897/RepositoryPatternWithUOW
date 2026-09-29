using Microsoft.EntityFrameworkCore;
using RepositoryPatternWithUOW.Core.Models;
using RepositoryPatternWithUOW.Core.Repository;

namespace RepositoryPatternWithUOW.Ef.Repository;

public class DepartmentRepository(AppDbContext context) : BaseRepository<Department>(context), IDepartmentRepository
{
    public async Task<List<Employee>> GetEmployeesOfDepartment(int id)
    {
        var res = await _context.Departments.
        Include(e => e.Employees).
        FirstOrDefaultAsync(e => e.Id == id);
        return res?.Employees ?? [];
    }
}
