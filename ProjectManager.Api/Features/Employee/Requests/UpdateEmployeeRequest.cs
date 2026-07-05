using ProjectManager.Entities.Enums;

namespace ProjectManager.Api.Features.Employee.Requests;

public class UpdateEmployeeRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Patronymic { get; set; }
    public string? Email { get; set; }
    public UserRole? Role { get; set; }
}