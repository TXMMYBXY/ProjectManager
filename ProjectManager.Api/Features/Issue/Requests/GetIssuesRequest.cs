using ProjectManager.Application.Issue;
using ProjectManager.Entities.Enums;

namespace ProjectManager.Api.Features.Issue.Requests;

public class GetIssuesRequest
{
    public IssueSortField? SortField { get; set; }
    public bool Descending { get; set; }
    
    public string? Title { get; set; }
    public IssueStatus? Status { get; set; }
    public byte? Priority { get; set; }
    public string? ProjectTitle { get; set; }
    public string? AuthorFullName { get; set; }
    public string? ExecutorFullName { get; set; }
    
    public int? PageSize { get; set; }
    public int? PageNumber { get; set; }
}