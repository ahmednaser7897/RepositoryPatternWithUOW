using RepositoryPatternWithUOW.Core.Models;
using RepositoryPatternWithUOW.Core.Repository;

namespace RepositoryPatternWithUOW.Ef.Repository;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    public IBaseRepository<Employee, int> Employees { get; private set; }

    public IDepartmentRepository<Department, int> Departments { get; private set; }


    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Employees = new BaseRepository<Employee, int>(_context);
        //Departments = new BaseRepository<Department>(_context);
        Departments = new DepartmentRepository<Department, int>(_context);
    }
    public Task<int> Complete()
    {
        return _context.SaveChangesAsync();
    }

    public virtual void Dispose()
    {
        // Prevent multiple disposes
        GC.SuppressFinalize(this);

        // Dispose the DbContext
        _context.Dispose();
    }
}
