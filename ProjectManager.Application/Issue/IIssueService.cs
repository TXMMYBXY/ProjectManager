using ProjectManager.Application.Issue.Dto;
using ProjectManager.Application.Utils;

namespace ProjectManager.Application.Issue;

public interface IIssueService
{
    Task<PagedIssueDto> GetAllIssuesAsync(IssueFilter filter, CurrentUser currentUser);
    Task<IssueInfoDto> GetIssueByIdAsync(int id, CurrentUser currentUser);
    Task CreateIssueAsync(CreateIssueDto dto);
    Task<IssueInfoDto> UpdateIssueAsync(int issueId, UpdateIssueDto dto);
    Task<bool> DeleteIssueByIdAsync(int id);
    Task<int> BulkDeleteIssuesAsync(IReadOnlyList<int> ids);
}