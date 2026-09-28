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
    private readonly IBaseRepository<Department> Repository;
    public DepartmentController(IBaseRepository<Department> repository)
    {
        Repository = repository;
    }

    [HttpGet()]
    public async Task<IActionResult> GetAlldepartment()
    {
        var departments = await Repository.GetAll();
        var dept = departments.Select(
            (s) => new DeptWithEmpDTO
            {
                Name = s.Name,
                ManagerName = s.ManagerName,
                EmpCount = s.Employees?.Count ?? 0
            });
        return Ok(dept);

    }


    [HttpGet]
    [Route("{id:int}")]//api/department/"id"
    public async Task<IActionResult> GetByDepartmentId(int id)
    {
        var department = await Repository.GetById(id);
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
        var department = await Repository.FindAsync(e => e.Name.Contains(name), ["Employees"]);
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
        await Repository.Add(department);
        await Repository.SaveChangesAsync();
        var model = GetByDepartmentId(department.Id);
        return CreatedAtAction(nameof(GetByDepartmentId), new { id = department.Id }, model);
    }
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateDepartment(int id, Department department)
    {
        var deptFromDb = await Repository.GetById(id);
        if (deptFromDb != null)
        {
            deptFromDb.Name = department.Name;
            deptFromDb.ManagerName = department.ManagerName;
            await Repository.Update(deptFromDb);
            await Repository.SaveChangesAsync();
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
        var isDeleted = await Repository.Remove(id);
        if (isDeleted)
        {
            await Repository.SaveChangesAsync();
            return NoContent();
        }
        else
        {
            return NotFound($"department with id {id} is not found");
        }
    }

}
