
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepositoryPatternWithUOW.Core.DTO;
using RepositoryPatternWithUOW.Core.Models;
using RepositoryPatternWithUOW.Core.Repository;
using RepositoryPatternWithUOW.Core.Specification;

namespace RepositoryPatternWithUOW.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpecificationController(IUnitOfWork unitOfWork) : ControllerBase
{
    private readonly IUnitOfWork UnitOfWork = unitOfWork;

    [HttpGet]
    public async Task<ActionResult<GenralResponse>> GetAllEmployee()
    {
        var specifications = new BaseSpecifications<Employee, int>();
        specifications.AddInclude(e => e.Department);
        specifications.AddPagination(5, 1);
        specifications.AddOrderBy(e => e.Salary);
        specifications.AddCriteria(e => e.Salary > 1000);
        var allEmployees = await UnitOfWork.Employees.GetAllWithSpec(specifications);
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
        var employee = await UnitOfWork.Employees.GetById(id);
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
        var employee = await UnitOfWork.Employees.FindAsync(e => e.Name.Contains(name), ["Department"]);

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
}
