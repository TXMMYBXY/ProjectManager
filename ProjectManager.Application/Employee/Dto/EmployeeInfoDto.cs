using ProjectManager.Application.Issue.Dto;
using ProjectManager.Application.Project.Dto;

namespace ProjectManager.Application.Employee.Dto;

public class EmployeeInfoDto
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? Patronymic { get; set; }
    public string Email { get; set; }

    public ICollection<ProjectItemDto> Projects { get; set; }
    public ICollection<IssueItemDto> AuthoredIssues { get; set; }
    public ICollection<IssueItemDto> ExecutedIssues { get; set; }
}
