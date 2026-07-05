using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ProjectManager.Entities.Enums;

namespace ProjectManager.Api.Features.Employee.Requests;

public class CreateEmployeeRequest
{
    [Required]
    [JsonPropertyName("firstName")]
    public string FirstName { get; set; }
    [Required]
    [JsonPropertyName("lastName")]
    public string LastName { get; set; }
    [JsonPropertyName("patronymic")]
    public string? Patronymic { get; set; }
    [Required]
    [JsonPropertyName("email")]
    public string Email { get; set; }
    [JsonPropertyName("role")]
    public UserRole Role { get; set; }
}