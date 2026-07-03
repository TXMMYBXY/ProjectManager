using AutoMapper;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Issue;
using ProjectManager.Application.Issue.Dto;

namespace ProjectManager.Infrastructure.Issue;

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

    public async Task<PagedIssueDto> GetAllIssuesAsync(IssueFilter filter)
    {
        var issuesDto = await _issueRepository.GetAllIssuesAsync(filter);

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
        var issueDto = await _issueRepository.GetIssueByIdAsync(id);
        
        ArgumentNullException.ThrowIfNull(issueDto, "Issue not found");

        return issueDto;
    }

    public async Task CreateIssueAsync(CreateIssueDto dto)
    {
        var issue = _mapper.Map<Entities.Models.Issue>(dto);
        
        await _issueRepository.CreateAsync(issue);
        await _issueRepository.SaveChangesAsync();
    }

    public async Task<IssueInfoDto> UpdateIssueAsync(int issueId, UpdateIssueDto dto)
    {
        var issue = await _issueRepository.GetByIdAsync(issueId);

        ArgumentNullException.ThrowIfNull(issue, "Issue not found");
        
        _mapper.Map(dto, issue);
        
        await _issueRepository.SaveChangesAsync();

        return _mapper.Map<IssueInfoDto>(issue);
    }

    public async Task<bool> DeleteIssueByIdAsync(int id)
    {
        return await _issueRepository.DeleteByIdAsync(id) > 0;
    }

    public async Task<int> BulkDeleteIssuesAsync(IReadOnlyList<int> ids)
    {
        return await _issueRepository.BulkDeleteAsync(ids);
    }
}