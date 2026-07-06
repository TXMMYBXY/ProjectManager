using System.Linq.Expressions;
using AutoMapper;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Common;
using ProjectManager.Application.Common.Exceptions;
using ProjectManager.Application.Issue;
using ProjectManager.Application.Issue.Dto;
using ProjectManager.Entities.Enums;

namespace ProjectManager.Infrastructure.Issue;

public class IssueService : IIssueService
{
    private readonly ILogger<IssueService> _logger;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;
    private readonly IIssueRepository _issueRepository;

    public IssueService(
        ILogger<IssueService> logger,
        IMapper mapper,
        ICurrentUser currentUser,
        IIssueRepository issueRepository)
    {
        _logger = logger;
        _mapper = mapper;
        _currentUser = currentUser;
        _issueRepository = issueRepository;
    }

    public async Task<PagedIssueDto> GetAllIssuesAsync(IssueFilter filter)
    {
        Expression<Func<Entities.Models.Issue, bool>>? predicate = null;

        if (_currentUser.IsInRole(nameof(UserRole.Employee)))
        {
            predicate = p => p.ExecutorId == _currentUser.Id;
        }
        else if (_currentUser.IsInRole(nameof(UserRole.Manager)))
        {
            predicate = p => p.AuthorId == _currentUser.Id || p.ExecutorId == _currentUser.Id;
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

    public async Task<IssueInfoDto> GetIssueByIdAsync(int id)
    {
        Expression<Func<Entities.Models.Issue, bool>>? predicate = null;

        if (_currentUser.IsInRole(nameof(UserRole.Employee)))
        {
            predicate = p => p.ExecutorId == _currentUser.Id;
        }
        else if (_currentUser.IsInRole(nameof(UserRole.Manager)))
        {
            predicate = p => p.AuthorId == _currentUser.Id || p.ExecutorId == _currentUser.Id;
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