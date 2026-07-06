using ProjectManager.Entities.Enums;

namespace ProjectManager.Application.Account.Dto;

public class RegisterDto
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? Patronymic { get; set; }
    public UserRole Role { get; set; } = UserRole.Employee;
    public string UserName => Email;
}