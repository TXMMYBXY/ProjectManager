using ProjectManager.Application.Issue.Dto;
using ProjectManager.Application.Project.Dto;

namespace ProjectManager.Api.Features.Employee.Responses;

public class EmployeeInfoResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? Patronymic { get; set; }
    public string Email { get; set; }

    public IReadOnlyList<ProjectItemDto> Projects { get; set; }
    public IReadOnlyList<IssueItemDto> AuthoredIssues { get; set; }
    public IReadOnlyList<IssueItemDto> ExecutedIssues { get; set; }
}