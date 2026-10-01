/*
    What is Specification Pattern in c# .NET ?
     In .NET, the Specification Pattern is an abstraction layer that separates business logic from data access logic. 
     It allows you to define business rules and data retrieval criteria in a reusable and maintainable way.
    
    Why do we need it ?
    1. Decoupling - It decouples the business logic from the data access logic, making the code more maintainable and testable.
    2. Testability - It makes the code more testable by allowing you to test the business logic and data access logic separately.
    3. Maintainability - It makes the code more maintainable by allowing you to change the data access logic without affecting the business logic.
    4. Reusability - It makes the code more reusable by allowing you to reuse the business logic and data access logic in different parts of the application.
    5. Scalability - It makes the code more scalable by allowing you to scale the business logic and data access logic independently.
    
   
    //---------------------------------------------------------------
    Repository Pattern Steps :
    1. Create Entity/Entities with Interface IEntity<TKey>
    this help us to genric repository with table and id type
    public interface IEntity<TKey>
    {
        public TKey Id { get; set; }
    }
    public class Employee : IEntity<int>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Salary { get; set; }
        public string JopTitle { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public string Address { get; set; } = null!;
        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }
    }
    2. Create Specification Interface
    public interface ISpecification<TEntity, TKey> where TEntity : IEntity<TKey>
    {
        Expression<Func<TEntity, bool>>? Criteria { get; }
        List<Expression<Func<TEntity, object>>> Includes { get; }
        List<OrderExpression<TEntity>> OrderBy { get; }
        int? Take { get; }//how many records to take
        int? Skip { get; }//how many records to skip
        bool IsPaginationEnabled { get; }
    }

    3. Implement the Genaric Repository interface
   public abstract class BaseSpecifications<TEntity, TKey>
    : ISpecification<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    {
        //this is the default spec = get all entities
        public BaseSpecifications()
        {
            Criteria = _ => true;
        }
        //Applay where condition
        public Expression<Func<TEntity, bool>>? Criteria { get; private set; }
        public void AddCriteria(Expression<Func<TEntity, bool>> criteria)
        {
            //custom spec = user criteria
            Criteria = criteria;
        }

        public List<Expression<Func<TEntity, object>>> Includes { get; } = [];
        public void AddInclude(Expression<Func<TEntity, object>> include)
        {
            Includes.Add(include);
        }
        //Applay order by
        public List<OrderExpression<TEntity>> OrderBy { get; } = [];

        public void AddOrderBy(Expression<Func<TEntity, object>> orderBy, bool isdesc = false)
        {
            OrderBy.Add(new OrderExpression<TEntity> { OrderBy = orderBy, IsDescending = isdesc });
        }
        //Apply pagination
        public int? Take { get; private set; }
        public int? Skip { get; private set; }
        public bool IsPaginationEnabled { get; private set; }
        public void AddPagination(int PageSize, int PageNumber)// Pagesize=10,PageNumber=3 ==> Skip=20,Take=10
        {
            Skip = PageSize * (PageNumber - 1);
            Take = PageSize;
            IsPaginationEnabled = true;
        }

    }
    This is a generic base repository that implements CRUD operations for any entity.
    4. Genrate SpecificationEvaluator
    public static class SpecificationEvaluator
    {
        public static IQueryable<TEntity> GenerateQuery<TEntity, TKey>(
            IQueryable<TEntity> inputQueryable,
            ISpecification<TEntity, TKey> specification
        )
        where TEntity : class, IEntity<TKey>
        {
            var queryable = inputQueryable;

            //apply criteria
            if (specification.Criteria != null)
            {
                //Select * from Product where Name="productName"
                queryable = queryable.Where(specification.Criteria);
            }
            //apply includes
            if (specification.Includes?.Count > 0)
            {
                //select * from product p join productcategory pc on p.productcategoryid = pc.id
                foreach (var include in specification.Includes)
                {
                    queryable = queryable.Include(include);
                }
            }
            //apply order by
            if (specification.OrderBy.Count > 0)
            {
                //1- get the first order by expression
                OrderExpression<TEntity> firstOrder = specification.OrderBy[0];
                //2- check id the order is descending or ascending and then order by
                IOrderedQueryable<TEntity> orderedQueryable =
                firstOrder.IsDescending ? queryable.OrderByDescending(firstOrder.OrderBy)
                : queryable.OrderBy(firstOrder.OrderBy);
                //2- get the rest of the order by expressions
                var remainingOrders = specification.OrderBy.Skip(1);
                foreach (var order in remainingOrders)
                {
                    orderedQueryable = order.IsDescending ?
                    orderedQueryable.OrderByDescending(order.OrderBy)
                    : orderedQueryable.OrderBy(order.OrderBy);
                }
                queryable = orderedQueryable;
            }
            //apply pagination
            if (specification.IsPaginationEnabled)
            {
                queryable = queryable.Skip(specification.Skip ?? 0).Take(specification.Take ?? 0);
            }
            return queryable;
        }
    }
     5. Create a Genaric Repository interface
    public interface IBaseRepository<TEntity, TKey> where TEntity : IEntity<TKey>
    {
        Task<TEntity?> GetByIdWithSpec(ISpecification<TEntity, TKey> spec);
        Task<List<TEntity>> GetAllWithSpec(ISpecification<TEntity, TKey> spec);
    }
    6- Inject In the UnitOfWork
     public IBaseRepository<Employee, int> Employees { get; private set; }
    public IDepartmentRepository<Department, int> Departments { get; private set; }
    
   7- Create Custom Specification
   class GetAllEmployeesSpec : BaseSpecifications<Employee, int>
    {
        public GetAllEmployeesSpec() : base()
        {
            AddInclude(e => e.Department);
            AddPagination(5, 1);
            AddOrderBy(e => e.Salary);
            AddCriteria(e => e.Salary > 1000);
        }
    }
    8- use it in the controller like this:
    [ApiController]
    [Route("api/[controller]")]
    public class SpecificationController(IUnitOfWork unitOfWork) : ControllerBase
    {
        private readonly IUnitOfWork UnitOfWork = unitOfWork;

        [HttpGet]
        public async Task<ActionResult<GenralResponse>> GetAllEmployee()
        {
            var allEmployees = await UnitOfWork.Employees.GetAllWithSpec(new GetAllEmployeesSpec());
            return Ok(new GenralResponse
            {
                Data = allEmployees,
                IsSuccess = true,
                Message = "Employees retrieved successfully",
                StatusCode = 200
            });
        }
    }
   
        
*/
