namespace RepositoryPatternWithUOW.Core.Models;

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? ManagerName { get; set; }

    public List<Employee>? Employees { get; set; }


    public override string ToString()
    {
        return $"Department info ==> Id: {Id}, Name: {Name}, ManagerName: {ManagerName}";
    }

}

