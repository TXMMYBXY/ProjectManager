using ProjectManager.Application.Common;
using ProjectManager.Application.Issue.Dto;

namespace ProjectManager.Application.Issue;

public interface IIssueRepository : IBaseRepository<Entities.Models.Issue>
{
    Task<(IReadOnlyList<IssueItemDto> Issues, int Count)> GetAllIssuesAsync(IssueFilter filter);
    Task<IssueInfoDto?> GetIssueByIdAsync(int issueId);
}