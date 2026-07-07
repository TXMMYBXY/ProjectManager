using ProjectManager.Application.Utils;
using ProjectManager.Entities.Enums;

namespace ProjectManager.Application.Issue.Dto;

public class UpdateIssueDto
{
    public string? Title { get; set; }
    public IssueStatus? Status { get; set; }
    public Optional<string?> Comments { get; set; }
    public byte? Priority { get; set; }
    public int? AuthorId { get; set; }
    public int? ExecutorId { get; set; }
}