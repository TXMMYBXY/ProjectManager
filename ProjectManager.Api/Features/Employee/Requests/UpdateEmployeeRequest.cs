using System.Text.Json.Serialization;
using ProjectManager.Entities.Enums;

namespace ProjectManager.Api.Features.Employee.Requests;

public class UpdateEmployeeRequest
{
    [JsonPropertyName("firstName")]
    public string? FirstName { get; set; }
    [JsonPropertyName("lastName")]
    public string? LastName { get; set; }
    [JsonPropertyName("patronymic")]
    public string? Patronymic { get; set; }
    [JsonPropertyName("email")]
    public string? Email { get; set; }
    [JsonPropertyName("role")]
    public UserRole? Role { get; set; }
}