using Microsoft.AspNetCore.Mvc;
using RepositoryPatternWithUOW.Core.DTO;
using RepositoryPatternWithUOW.Core.Models;
using RepositoryPatternWithUOW.Core.Repository;

namespace RepositoryPatternWithUOW.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
//http://localhost:5260/swagger/index.html
public class DepartmentController : ControllerBase
{
    //Instead of using RepositoryPattern and injecting many repositories 
    // we will use Unit of Work Pattern with Repository Pattern
    //private readonly IBaseRepository<Department> DepartmentRepository;
    //private readonly IBaseRepository<Employee> EmployeeRepository;
    private readonly IUnitOfWork UnitOfWork;
    public DepartmentController(
        // IBaseRepository<Department> departmentRepository,
        // IBaseRepository<Employee> employeeRepository
        IUnitOfWork UnitOfWork)
    {
        // DepartmentRepository = departmentRepository;
        // EmployeeRepository = employeeRepository;
        this.UnitOfWork = UnitOfWork;
    }

    [HttpGet()]
    public async Task<IActionResult> GetAlldepartment()
    {
        var departments = await UnitOfWork.Departments.GetAll();
        var dept = departments.Select(
            (s) => new DeptWithEmpDTO
            {
                Name = s.Name,
                ManagerName = s.ManagerName,
                EmpCount = s.Employees?.Count ?? 0
            });
        return Ok(dept);

    }
    [HttpGet("GetAllEmployees")]
    public async Task<IActionResult> GetAllEmployees()
    {
        var employees = await UnitOfWork.Employees.GetAll();
        return Ok(employees);
    }

    [HttpGet("GetEmployeesOfDepartment")]
    public async Task<IActionResult> GetEmployeesOfDepartment(int id)
    {
        var employees = await UnitOfWork.Departments.GetEmployeesOfDepartment(id);
        return Ok(employees);
    }


    [HttpGet]
    [Route("{id:int}")]//api/department/"id"
    public async Task<IActionResult> GetByDepartmentId(int id)
    {
        var department = await UnitOfWork.Departments.GetById(id);
        if (department == null)
            return NotFound($"department with id {id} is not found");
        var dept = new DeptWithEmpDTO
        {
            Name = department.Name,
            ManagerName = department.ManagerName,
            EmpCount = department.Employees?.Count ?? 0
        };
        return Ok(dept);
    }
    [HttpGet]
    [Route("findByName")]//api/department/findByName?name=HR
    public async Task<IActionResult> FindByDepartmentName(string name)
    {
        var department = await UnitOfWork.Departments.FindAsync(e => e.Name.Contains(name), ["Employees"]);
        if (department == null)
            return NotFound($"department with name {name} is not found");
        var dept = new DeptWithEmpDTO
        {
            Name = department.Name,
            ManagerName = department.ManagerName,
            EmpCount = department.Employees?.Count ?? 0
        };
        return Ok(dept);
    }


    [HttpPost]
    public async Task<IActionResult> Addepartment(Department department)
    {
        await UnitOfWork.Departments.Add(department);
        await UnitOfWork.Complete();
        var model = GetByDepartmentId(department.Id);
        return CreatedAtAction(nameof(GetByDepartmentId), new { id = department.Id }, model);
    }
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateDepartment(int id, Department department)
    {
        var deptFromDb = await UnitOfWork.Departments.GetById(id);
        if (deptFromDb != null)
        {
            deptFromDb.Name = department.Name;
            deptFromDb.ManagerName = department.ManagerName;
            await UnitOfWork.Departments.Update(deptFromDb);
            await UnitOfWork.Complete();
            return NoContent();
        }
        else
        {
            return NotFound($"department with id {id} is not found");
        }

    }
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteDepartment(int id)
    {
        var isDeleted = await UnitOfWork.Departments.Remove(id);
        if (isDeleted)
        {
            await UnitOfWork.Complete();
            return NoContent();
        }
        else
        {
            return NotFound($"department with id {id} is not found");
        }
    }

}
