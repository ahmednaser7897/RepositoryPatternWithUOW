using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepositoryPatternWithUOW.Core.DTO;
using RepositoryPatternWithUOW.Core.Models;
using RepositoryPatternWithUOW.Core.Repository;

namespace RepositoryPatternWithUOW.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
//http://localhost:5260/swagger/index.html
public class EmployeeController(IBaseRepository<Employee> repository) : ControllerBase
{
    private readonly IBaseRepository<Employee> Repository = repository;

    //  http://localhost:5260/api/Employee/

    [HttpGet]
    public async Task<ActionResult<GenralResponse>> GetAllEmployee()
    {

        var allEmployees = await Repository.GetAll();
        return Ok(new GenralResponse
        {
            Data = allEmployees,
            IsSuccess = true,
            Message = "Employees retrieved successfully",
            StatusCode = 200
        });
    }

    [HttpGet]
    [Route("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<GenralResponse>> GetByEmployeeId(int id)
    {
        var employee = await Repository.GetById(id);
        GenralResponse response;
        if (employee != null)
        {
            response = new GenralResponse
            {
                Data = employee,
                IsSuccess = true,
                Message = "Employee retrieved successfully",
                StatusCode = 200
            };
        }
        else
        {
            response = new GenralResponse
            {
                Data = null,
                IsSuccess = false,
                Message = "Employee not found",
                StatusCode = 404
            };
        }
        return Ok(response);
    }
    [HttpGet]
    [Route("findByName")]//api/department/findByName?name=HR
    public async Task<IActionResult> FindByEmployeeName(string name)
    {
        var employee = await Repository.FindAsync(e => e.Name.Contains(name), ["Department"]);

        if (employee == null)
            return NotFound($"Employee with name {name} is not found");
        return Ok(new GenralResponse
        {
            Data = employee,
            IsSuccess = true,
            Message = "Employee retrieved successfully",
            StatusCode = 200
        });
    }


    [HttpPost]
    public async Task<IActionResult> AdEmployee(Employee Employee)
    {
        await Repository.Add(Employee);
        await Repository.SaveChangesAsync();
        var model = await GetByEmployeeId(Employee.Id);
        //ex: location: http://localhost:5260/api/Employee/6
        return CreatedAtAction(nameof(GetByEmployeeId), new { id = Employee.Id }, model);
    }
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateEmployee(int id, Employee Employee)
    {
        var deptFromDb = await Repository.GetById(id);
        if (deptFromDb != null)
        {
            deptFromDb.Name = Employee.Name;
            deptFromDb.Address = Employee.Address;
            deptFromDb.Salary = Employee.Salary;
            deptFromDb.JopTitle = Employee.JopTitle;
            deptFromDb.ImageUrl = Employee.ImageUrl;
            await Repository.Update(deptFromDb);
            await Repository.SaveChangesAsync();
            return NoContent();
        }
        else
        {
            return NotFound($"Employee with id {id} is not found");
        }

    }
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var isDeleted = await Repository.Remove(id);
        if (isDeleted)
        {
            await Repository.SaveChangesAsync();
            return NoContent();
        }
        else
        {
            return NotFound($"Employee with id {id} is not found");
        }
    }

    [HttpGet]
    [Route("findAllEmployees")]//api/department/findByName?name=HR
    public async Task<ActionResult<GenralResponse>> FindAllEmployees()
    {
        var allEmployees = await Repository.FindAllAsync(
            predicate: e => e.Salary > 1000, ["Department"],
            take: 4, skip: 2, orderBy: e => e.Salary, isAscending: false);
        return Ok(new GenralResponse
        {
            Data = allEmployees,
            IsSuccess = true,
            Message = "Employees retrieved successfully",
            StatusCode = 200
        });
    }

}
