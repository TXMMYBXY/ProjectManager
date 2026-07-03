using ProjectManager.Application.Issue.Dto;

namespace ProjectManager.Application.Issue;

public interface IIssueService
{
    Task<PagedIssueDto> GetAllIssuesAsync(IssueFilter filter);
    Task<IssueInfoDto> GetIssueByIdAsync(int id);
    Task CreateIssueAsync(CreateIssueDto dto);
    Task<IssueInfoDto> UpdateIssueAsync(int issueId, UpdateIssueDto dto);
    Task<bool> DeleteIssueByIdAsync(int id);
    Task<int> BulkDeleteIssuesAsync(IReadOnlyList<int> ids);
}