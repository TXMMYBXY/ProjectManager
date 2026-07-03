using ProjectManager.Application.Common;

namespace ProjectManager.Application.Issue.Dto;

public class PagedIssueDto : PagedData
{
    public IReadOnlyList<IssueItemDto> Issues { get; set; }
}