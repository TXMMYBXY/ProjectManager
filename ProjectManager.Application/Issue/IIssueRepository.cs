using System.Linq.Expressions;
using ProjectManager.Application.Common;
using ProjectManager.Application.Issue.Dto;

namespace ProjectManager.Application.Issue;

public interface IIssueRepository : IBaseRepository<Entities.Models.Issue>
{
    Task<(IReadOnlyList<IssueItemDto> Issues, int Count)> GetAllIssuesAsync(IssueFilter filter, 
        Expression<Func<Entities.Models.Issue, bool>>? predicate = null);
    Task<IssueInfoDto?> GetIssueByIdAsync(int issueId, 
        Expression<Func<Entities.Models.Issue, bool>>? predicate = null);
}