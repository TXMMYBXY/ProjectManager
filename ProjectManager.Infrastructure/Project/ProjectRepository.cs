using Microsoft.EntityFrameworkCore;
using ProjectManager.Application.Employee.Dto;
using ProjectManager.Application.Issue.Dto;
using ProjectManager.Application.Project;
using ProjectManager.Application.Project.Dto;
using ProjectManager.Infrastructure.Common;
using ProjectManager.Infrastructure.Data;

namespace ProjectManager.Infrastructure.Project;

public class ProjectRepository : BaseRepository<Entities.Models.Project>, IProjectRepository
{
    private readonly ApplicationDbContext _dbContext;
    
    public ProjectRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(IReadOnlyList<ProjectItemDto>, int)> GetAllProjectsAsync(ProjectFilter filter)
    {
        var query = _dbContext.Projects
            .AsNoTracking()
            .ApplyFilter(filter)
            .ApplySorting(filter)
            .AsQueryable();
        
        var totalCount = await query.CountAsync();

        var result = query
            .Select(p => new ProjectItemDto
            {
                Id = p.Id,
                Title = p.Title,
                CompanyCustomer = p.CompanyCustomer,
                CompanyExecuter = p.CompanyExecuter,
                StartDate = p.StartDate,
                FinishDate = p.FinishDate,
                Priority = p.Priority,
                ProjectManager = new EmployeeSummaryDto
                {
                    Id = p.ProjectManager.Id,
                    FirstName = p.ProjectManager.FirstName,
                    LastName = p.ProjectManager.LastName,
                    Patronymic =  p.ProjectManager.Patronymic,
                    Email = p.ProjectManager.Email
                },
                
            })
            .ApplyPagination(filter);
        
        return (await result.ToListAsync(), totalCount);
    }

    public async Task<ProjectInfoDto?> GetProjectByIdAsync(int projectId)
    {
        return await _dbContext.Projects
            .AsNoTracking()
            .Where(p => p.Id == projectId)
            .Select(p => new ProjectInfoDto
            {
                Id = p.Id,
                Title = p.Title,
                CompanyCustomer = p.CompanyCustomer,
                CompanyExecuter = p.CompanyExecuter,
                StartDate = p.StartDate,
                FinishDate = p.FinishDate,
                Priority = p.Priority,
                ProjectManager = new EmployeeSummaryDto
                {
                    Id = p.ProjectManager.Id,
                    FirstName = p.ProjectManager.FirstName,
                    LastName = p.ProjectManager.LastName,
                    Patronymic = p.ProjectManager.Patronymic,
                    Email = p.ProjectManager.Email
                },
                Employees = p.EmployeeProjects.Select(ep => new EmployeeItemDto
                {
                    Id = ep.Employee.Id,
                    FirstName = ep.Employee.FirstName,
                    LastName = ep.Employee.LastName,
                    Patronymic = ep.Employee.Patronymic,
                    Email = ep.Employee.Email
                }).ToList(),
                Issues = p.Issues.Select(i => new IssueItemDto
                {
                    Id = i.Id,
                    Title = i.Title,
                    Status = i.Status,
                    Comments = i.Comments,
                    Priority = i.Priority,
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

                }).ToList()
            })
            .SingleOrDefaultAsync();
    }
}