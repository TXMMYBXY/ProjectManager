using Microsoft.EntityFrameworkCore;
using ProjectManager.Application.Employee.Dto;
using ProjectManager.Application.Issue;
using ProjectManager.Application.Issue.Dto;
using ProjectManager.Application.Project.Dto;
using ProjectManager.Infrastructure.Common;
using ProjectManager.Infrastructure.Data;

namespace ProjectManager.Infrastructure.Issue;

public class IssueRepository : BaseRepository<Entities.Models.Issue>, IIssueRepository
{
    private readonly ApplicationDbContext _dbContext;
    
    public IssueRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(IReadOnlyList<IssueItemDto>, int)> GetAllIssuesAsync(IssueFilter filter)
    {
        var query = _dbContext.Issues
            .AsNoTracking()
            .ApplyFilter(filter)
            .ApplySorting(filter)
            .AsQueryable(); 
        
        var totalCount = await query.CountAsync();

        var result = query
            .Select(i => new IssueItemDto
            {
                Id = i.Id,
                Title = i.Title,
                Status = i.Status,
                Comments = i.Comments,
                Priority = i.Priority,
                Project = new ProjectSummaryDto
                {
                    Id = i.Project.Id,
                    Title = i.Project.Title,
                    CompanyCustomer = i.Project.CompanyCustomer,
                    CompanyExecuter = i.Project.CompanyExecutor,
                    StartDate = i.Project.StartDate,
                    FinishDate = i.Project.FinishDate,
                    Priority = i.Project.Priority
                },
                Author = new EmployeeSummaryDto
                {
                    Id = i.Author.Id,
                    FirstName = i.Author.FirstName,
                    LastName = i.Author.LastName,
                    Patronymic = i.Author.Patronymic,
                    Email = i.Author.Email
                },
                Executor = new EmployeeSummaryDto
                {
                    Id = i.Executor.Id,
                    FirstName = i.Executor.FirstName,
                    LastName = i.Executor.LastName,
                    Patronymic = i.Executor.Patronymic,
                    Email = i.Executor.Email
                }
            });

        return (await result.ToListAsync(), totalCount);
    }

    public async Task<IssueInfoDto?> GetIssueByIdAsync(int issueId)
    {
        return await _dbContext.Issues
            .Where(i => i.Id == issueId)
            .Select(i => new IssueInfoDto
            {
                Id = i.Id,
                Title = i.Title,
                Status = i.Status,
                Comments = i.Comments,
                Priority = i.Priority,
                Project = new ProjectSummaryDto
                {
                    Id = i.Project.Id,
                    Title = i.Project.Title,
                    CompanyCustomer = i.Project.CompanyCustomer,
                    CompanyExecuter = i.Project.CompanyExecutor,
                    StartDate = i.Project.StartDate,
                    FinishDate = i.Project.FinishDate,
                    Priority = i.Project.Priority
                },
                Author = new EmployeeSummaryDto
                {
                    Id = i.Author.Id,
                    FirstName = i.Author.FirstName,
                    LastName = i.Author.LastName,
                    Patronymic = i.Author.Patronymic,
                    Email = i.Author.Email
                },
                Executor = new EmployeeSummaryDto
                {
                    Id = i.Executor.Id,
                    FirstName = i.Executor.FirstName,
                    LastName = i.Executor.LastName,
                    Patronymic = i.Executor.Patronymic,
                    Email = i.Executor.Email
                }
            })
            .SingleOrDefaultAsync();
    }
}