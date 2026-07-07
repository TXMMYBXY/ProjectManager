using System.Text.Json.Serialization;
using ProjectManager.Application.Employee.Dto;

namespace ProjectManager.Api.Features.Employee.Responses;

public class ProjectManagerResponse
{
    [JsonPropertyName("projectManagers")]
    public IReadOnlyList<EmployeeItemDto> ProjectManagers { get; set; }
}