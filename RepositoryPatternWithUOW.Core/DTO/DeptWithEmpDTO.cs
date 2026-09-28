namespace RepositoryPatternWithUOW.Core.DTO;

public class DeptWithEmpDTO
{
    public string Name { get; set; } = null!;
    public string? ManagerName { get; set; }

    public int EmpCount { get; set; }
}
