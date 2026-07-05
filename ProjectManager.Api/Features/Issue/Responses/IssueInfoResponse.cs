using ProjectManager.Application.Employee.Dto;
using ProjectManager.Application.Project.Dto;
using ProjectManager.Entities.Enums;

namespace ProjectManager.Api.Features.Issue.Responses;

public class IssueInfoResponse
{
    public int Id { get; set; }
    public string Title { get; set; }
    public IssueStatus Status { get; set; }
    public string? Comments { get; set; }
    public byte Priority { get; set; }
    public ProjectSummaryDto Project { get; set; }
    public EmployeeSummaryDto Author { get; set; }
    public EmployeeSummaryDto Executor { get; set; }
}