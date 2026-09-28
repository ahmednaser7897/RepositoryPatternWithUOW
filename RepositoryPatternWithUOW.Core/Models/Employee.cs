using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace RepositoryPatternWithUOW.Core.Models;

public class Employee
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
    [MinLength(3, ErrorMessage = "Name must be at least 3 characters long")]
    public string Name { get; set; } = null!;
    [Required(ErrorMessage = "Salary is required")]
    [Range(1000, 100000, ErrorMessage = "Salary must be between 1000 and 100000")]

    public decimal Salary { get; set; }
    public string JopTitle { get; set; } = null!;
    public string? ImageUrl { get; set; }
    [Required(ErrorMessage = "Address is required")]
    [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "Address can only contain letters and spaces")]
    public string Address { get; set; } = null!;
    public int? DepartmentId { get; set; }
    [JsonIgnore]
    public Department? Department { get; set; }
    public override string ToString()
    {
        return $"Id: {Id}, Name: {Name}, Salary: {Salary}, JopTitle: {JopTitle}, ImageUrl: {ImageUrl}, Address: {Address}, DepartmentId: {DepartmentId}, Department: {Department}";
    }
}
