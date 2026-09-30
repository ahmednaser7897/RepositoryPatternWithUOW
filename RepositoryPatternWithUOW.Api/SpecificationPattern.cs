/*
    What is Specification Pattern in c# .NET ?
    The Specification Pattern is a design pattern that abstracts the data access logic from the business logic.
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
