using ProjectManager.Entities.Enums;

namespace ProjectManager.Api.Features.Issue.Responses;

public class UpdateIssueResponse
{
    public string? Title { get; set; }
    public IssueStatus? Status { get; set; }
    public string? Comments { get; set; }
    public byte? Priority = 1;
    public int? AuthorId { get; set; }
    public int? ExecutorId { get; set; }
}