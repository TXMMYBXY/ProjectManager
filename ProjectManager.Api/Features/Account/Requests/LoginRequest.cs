using System.ComponentModel.DataAnnotations;

namespace ProjectManager.Api.Features.Account.Requests;

public class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}