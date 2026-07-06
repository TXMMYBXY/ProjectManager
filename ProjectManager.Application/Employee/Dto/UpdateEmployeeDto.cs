using ProjectManager.Application.Utils;
using ProjectManager.Entities.Enums;

namespace ProjectManager.Application.Employee.Dto;

public class UpdateEmployeeDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public Optional<string?> Patronymic { get; set; }
    public string? Email { get; set; }
    public UserRole? Role { get; set; }
}