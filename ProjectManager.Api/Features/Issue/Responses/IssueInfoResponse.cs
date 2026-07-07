using System.Text.Json.Serialization;
using ProjectManager.Application.Employee.Dto;
using ProjectManager.Application.Project.Dto;
using ProjectManager.Entities.Enums;

namespace ProjectManager.Api.Features.Issue.Responses;

public class IssueInfoResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
    [JsonPropertyName("status")]
    public IssueStatus Status { get; set; }
    [JsonPropertyName("comments")]
    public string? Comments { get; set; }
    [JsonPropertyName("priority")]
    public byte Priority { get; set; }
    [JsonPropertyName("project")]
    public ProjectSummaryDto Project { get; set; } = null!;
    [JsonPropertyName("author")]
    public EmployeeSummaryDto Author { get; set; } = null!;
    [JsonPropertyName("executor")]
    public EmployeeSummaryDto Executor { get; set; } = null!;
}