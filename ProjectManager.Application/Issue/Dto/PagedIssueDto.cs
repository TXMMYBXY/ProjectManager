using ProjectManager.Application.Common;
using ProjectManager.Application.Utils;

namespace ProjectManager.Application.Issue.Dto;

public class PagedIssueDto : PagedData
{
    public IReadOnlyList<IssueItemDto> Issues { get; set; }
}