using System.ComponentModel.DataAnnotations;
using ProjectManager.Entities.Enums;

namespace ProjectManager.Api.Features.Employee.Requests;

public class CreateEmployeeRequest
{
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    public string? Patronymic { get; set; }
    [Required]
    public string Email { get; set; }
    public UserRole Role { get; set; }
}