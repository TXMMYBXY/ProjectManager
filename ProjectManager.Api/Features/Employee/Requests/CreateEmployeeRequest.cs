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
    [EmailAddress]
    [JsonPropertyName("email")]
    public string Email { get; set; }
    [Required]
    [JsonPropertyName("password")]
    public string Password { get; set; }
    [JsonPropertyName("role")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public UserRole Role { get; set; }
}