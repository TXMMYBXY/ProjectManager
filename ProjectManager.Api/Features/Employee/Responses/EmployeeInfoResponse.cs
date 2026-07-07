using System.Text.Json.Serialization;
using ProjectManager.Application.Issue.Dto;
using ProjectManager.Application.Project.Dto;

namespace ProjectManager.Api.Features.Employee.Responses;

public class EmployeeInfoResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = string.Empty;
    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = string.Empty;
    [JsonPropertyName("patronymic")]
    public string? Patronymic { get; set; }
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("projects")]
    public IReadOnlyList<ProjectItemDto> Projects { get; set; } = [];
    [JsonPropertyName("authoredIssues")]
    public IReadOnlyList<IssueItemDto> AuthoredIssues { get; set; } = [];
    [JsonPropertyName("executedIssues")]
    public IReadOnlyList<IssueItemDto> ExecutedIssues { get; set; } = [];
}