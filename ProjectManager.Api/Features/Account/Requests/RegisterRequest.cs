using System.ComponentModel.DataAnnotations;
using ProjectManager.Entities.Enums;

namespace ProjectManager.Api.Features.Account.Requests;

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public string ConfirmPassword { get; set; }
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    public string? Patronymic { get; set; }
    public UserRole Role { get; set; } = UserRole.Employee;
}