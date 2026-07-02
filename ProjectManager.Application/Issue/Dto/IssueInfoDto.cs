using ProjectManager.Application.Employee.Dto;
using ProjectManager.Application.Project.Dto;
using ProjectManager.Entities.Enums;

namespace ProjectManager.Application.Issue.Dto;

public class IssueInfoDto
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