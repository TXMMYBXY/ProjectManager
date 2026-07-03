using System.Diagnostics;
using ProjectManager.Entities.Enums;

namespace ProjectManager.Application.Issue.Dto;

public class CreateIssueDto
{
    public string Title { get; set; }
    public IssueStatus Status { get; set; } = IssueStatus.ToDo;
    public string? Comments { get; set; }
    public byte Priority = 1;
    public int ProjectId { get; set; }
    public int AuthorId { get; set; }
    public int ExecutorId { get; set; }
}