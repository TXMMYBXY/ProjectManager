using System.Linq.Expressions;
using AutoMapper;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Common;
using ProjectManager.Application.Common.Exceptions;
using ProjectManager.Application.Issue.Dto;
using ProjectManager.Application.Utils;
using ProjectManager.Entities.Enums;

namespace ProjectManager.Application.Issue;

public class IssueService : IIssueService
{
    private readonly ILogger<IssueService> _logger;
    private readonly IMapper _mapper;
    private readonly IIssueRepository _issueRepository;

    public IssueService(
        ILogger<IssueService> logger,
        IMapper mapper,
        IIssueRepository issueRepository)
    {
        _logger = logger;
        _mapper = mapper;
        _issueRepository = issueRepository;
    }

    public async Task<PagedIssueDto> GetAllIssuesAsync(IssueFilter filter, CurrentUser currentUser)
    {
        Expression<Func<Entities.Models.Issue, bool>>? predicate = null;

        if (currentUser.Role.Equals(nameof(UserRole.Employee)))
        {
            predicate = p => p.ExecutorId == currentUser.Id;
        }
        else if (currentUser.Role.Equals(nameof(UserRole.Manager)))
        {
            predicate = p => p.AuthorId == currentUser.Id || p.ExecutorId == currentUser.Id;
        }
        
        var issuesDto = await _issueRepository.GetAllIssuesAsync(filter, predicate);

        return new PagedIssueDto
        {
            Issues = issuesDto.Issues,
            TotalCount = issuesDto.Count,
            PageSize = filter.PageSize ?? issuesDto.Count,
            CurrentPage = filter.PageNumber ?? 1
        };
    }

    public async Task<IssueInfoDto> GetIssueByIdAsync(int id, CurrentUser currentUser)
    {
        Expression<Func<Entities.Models.Issue, bool>>? predicate = null;

        if (currentUser.Role.Equals(nameof(UserRole.Employee)))
        {
            predicate = p => p.ExecutorId == currentUser.Id;
        }
        else if (currentUser.Role.Equals(nameof(UserRole.Manager)))
        {
            predicate = p => p.AuthorId == currentUser.Id || p.ExecutorId == currentUser.Id;
        }
        
        var issueDto = await _issueRepository.GetIssueByIdAsync(id, predicate);
        
        NotFoundException.ThrowIfNull(issueDto, "Issue not found");

        return issueDto;
    }

    public async Task CreateIssueAsync(CreateIssueDto dto)
    {
        var issue = _mapper.Map<Entities.Models.Issue>(dto);
        
        await _issueRepository.CreateAsync(issue);
        await _issueRepository.SaveChangesAsync();
        
        _logger.LogInformation("Issue successfully created with id {0}", issue.Id);
    }

    public async Task<IssueInfoDto> UpdateIssueAsync(int issueId, UpdateIssueDto dto)
    {
        var issue = await _issueRepository.GetByIdAsync(issueId);

        NotFoundException.ThrowIfNull(issue, "Issue not found");
        
        _mapper.Map(dto, issue);
        
        if(dto.Comments.HasValue)
            issue.Comments = dto.Comments.Value;
        
        await _issueRepository.SaveChangesAsync();
        
        _logger.LogInformation("Issue successfully updated with id {0}", issue.Id);

        return _mapper.Map<IssueInfoDto>(issue);
    }

    public async Task<bool> DeleteIssueByIdAsync(int id)
    {
        var result = await _issueRepository.DeleteByIdAsync(id) > 0;

        if(result)
            _logger.LogInformation("Issue successfully deleted with id {0}", id);
        else
            throw new NotFoundException("Issue not found");
        
        return result;
    }

    public async Task<int> BulkDeleteIssuesAsync(IReadOnlyList<int> ids)
    {
        var result = await _issueRepository.BulkDeleteAsync(ids);

        _logger.LogInformation("Bulk delete issues successfully");

        return result;
    }
}