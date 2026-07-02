using ProjectManager.Entities.Enums;

namespace ProjectManager.Application.Issue;

public class IssueFilter
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