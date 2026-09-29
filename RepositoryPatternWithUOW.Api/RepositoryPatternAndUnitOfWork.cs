/*
    What is Repository Pattern in c# .NET ?
    The Repository Pattern is a design pattern that abstracts the data access logic from the business logic.
    It provides a way to access data from a database without exposing the details of the database to the business logic.
    Why do we need it ?
    1. Decoupling
    2. Testability
    3. Maintainability
    4. Scalability
    5. Security
    How to implement it ?
    1. Define an interface for the repository
    2. Create a class that implements the interface
    3. Register the repository with the dependency injection container
    4. Use the repository in the business logic
    //---------------------------------------------------------------
    Repository Pattern Steps :
    1. Define an interface for the repository
   
    will create a generic base repository for all entities that implements CRUD operations:
    public interface IBaseRepository<T>
    {
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(int id);
    }
    This is a generic base repository that implements CRUD operations for any entity.
    
    2. Create a class that implements the interface
    We must create a repository for each entity that inherits from the base repository:
    and if it has specific operations that are not in the base repository, we must add them to the base repository:
    public class BaseRepository<T> : IBaseRepository<T>
    {
        private readonly AppDbContext _context;
        public BaseRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }
        public async Task<T> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            return entity;
        }
        public async Task<T> UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            return entity;
        }
        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
            }
        }
    }
    
    3. Register the repository with the dependency injection container
    in the Program.cs file, we will register the repository with the dependency injection container:
    builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
    
    4. Use the repository in the business logic
    public class DepartmentController(IBaseRepository<Department> repository) : ControllerBase
    {
        private readonly IBaseRepository<Department> Repository = repository;
        [HttpGet]
        public async Task<ActionResult<GenralResponse>> GetAllDepartments()
        {
            var allDepartments = await Repository.GetAllAsync();
            return Ok(new GenralResponse
            {
                Data = allDepartments,
                IsSuccess = true,
                Message = "Departments retrieved successfully",
                StatusCode = 200
            });
        }
    }
    //If we have a specific operations for a specific entity, we must create a repository for that entity
    //We will create an interface for the department repository that inherits from the base repository:
    public interface IDepartmentRepository : IBaseRepository<Department>
    {
        public Task<List<Employee>> GetEmployeesOfDepartment(int id);
    }
    //We will create a class that implements the interface and inherit from the BaseRepository class:
    //so now wee will implement this methods in IDepartmentRepository
    // and will inherit all the methods from the BaseRepository class:
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
    //we will use it in the controller like this:
    public class DepartmentController(IDepartmentRepository repository) : ControllerBase
    {
        //private readonly IBaseRepository<Department> Repository = repository;
         private readonly IDepartmentRepository Repository = repository;
        [HttpGet]
        public async Task<ActionResult<GenralResponse>> GetAllDepartments()
        {
            var allDepartments = await Repository.GetAllAsync();
            return Ok(new GenralResponse
            {
                Data = allDepartments,
                IsSuccess = true,
                Message = "Departments retrieved successfully",
                StatusCode = 200
            });
        }
    }
        
*/
//-------------------------------------------------------------------------------------------------------------
/*
    What is Unit of Work?
    The Unit of Work Pattern is a design pattern that manages multiple data access operations as a single unit.
    It ensures that either all operations are completed successfully, or none of them are.
    Why do we need it ?
    1. Decoupling
    2. Testability
    3. Maintainability
    4. Scalability
    5. Security
    //---------------------------------------------------------------
    What are the benefits of using Unit of Work Pattern?
    1. Decoupling: It decouples the business logic from the data access layer.
    2. Testability: It makes the code more testable.
    3. Maintainability: It makes the code more maintainable.
    4. Scalability: It makes the code more scalable.
    5. Security: It makes the code more secure.
    how to register it with the dependency injection container ?
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    How to implement it ?
    1. Define an interface for the unit of work
    2.Create a class that implements the interface
    3.Register the unit of work with the dependency injection container
    4. Use the unit of work in the business logic
    //---------------------------------------------------------------
    Unit of Work Steps :
    1- in RepositoryPatternWithUOW.Core create new folder Repository and add interface of IUnitOfWork.cs and IBaseRepository.cs
    //this interface will be used to access all the repositories and will inherit from the IDisposable interface:
   public interface IUnitOfWork : IDisposable
    {
        public IBaseRepository<Employee> Employees { get; }
        public IDepartmentRepository Departments { get; }
        public Task<int> Complete();
    }
    2- We will create a class that implements the interface and inherit from the IDisposable interface:
    so now wee will implement this methods in IUnitOfWork
    and will inherit all the methods from the IDisposable interface:
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public IBaseRepository<Employee> Employees { get; private set; }
        //public IBaseRepository<Department> Departments{ get; private set; }
        public IDepartmentRepository Departments { get; private set; }
                
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Employees = new BaseRepository<Employee>(_context);
            //Departments = new BaseRepository<Department>(_context);
            Departments = new DepartmentRepository(_context);
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
        
    3-register the unit of work with the dependency injection container:
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        
    4-we will use it in the controller like this:
    public class DepartmentController(IUnitOfWork unitOfWork) : ControllerBase
    {
        private readonly IUnitOfWork UnitOfWork = unitOfWork;
        [HttpGet]
        public async Task<ActionResult<GenralResponse>> GetAllDepartments()
        {
            var allDepartments = await UnitOfWork.Departments.GetAllAsync();
            return Ok(new GenralResponse
            {
                Data = allDepartments,
                IsSuccess = true,
                Message = "Departments retrieved successfully",
                StatusCode = 200
            });
        }
        [HttpGet("GetAllEmployees")]
        public async Task<IActionResult> GetAllEmployees()
        {
            var employees = await UnitOfWork.Employees.GetAllAsync();
            return Ok(employees);
        }
    }
    The Complete method must be called after all the operations are completed:
    The Complete method will save all the changes to the database:
    The Complete method will return the number of affected rows:
    why we must call complete() ?
    Because the complete method is the one that will save all the changes to the database:
    

*/