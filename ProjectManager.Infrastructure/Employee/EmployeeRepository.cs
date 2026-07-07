using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ProjectManager.Application.Employee;
using ProjectManager.Application.Employee.Dto;
using ProjectManager.Application.Issue.Dto;
using ProjectManager.Application.Project.Dto;
using ProjectManager.Infrastructure.Common;
using ProjectManager.Infrastructure.Data;

namespace ProjectManager.Infrastructure.Employee;

public class EmployeeRepository : BaseRepository<Entities.Models.Employee>, IEmployeeRepository
{
    private readonly ApplicationDbContext _dbContext;

    public EmployeeRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(IReadOnlyList<EmployeeItemDto>, int)> GetAllEmployeesAsync(EmployeeFilter filter,
        Expression<Func<Entities.Models.Employee, bool>>? predicate = null)
    {
        var query = _dbContext.Employees.AsQueryable();

        if (predicate != null)
            query = query.Where(predicate);

        query = query
            .AsNoTracking()
            .ApplyFilter(filter)
            .ApplySorting(filter);

        var totalCount = await query.CountAsync();

        var result = query
            .Select(e => new EmployeeItemDto
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Patronymic = e.Patronymic,
                Email = e.Email
            })
            .ApplyPagination(filter);

        return (await result.ToListAsync(), totalCount);
    }

    public async Task<EmployeeInfoDto?> GetEmployeeByIdAsync(int employeeId,
        Expression<Func<Entities.Models.Employee, bool>>? predicate = null)
    {
        var query = _dbContext.Employees.AsQueryable();

        if (predicate != null)
            query = query.Where(predicate);

        return await query
            .AsNoTracking()
            .Where(e => e.Id == employeeId)
            .Select(e => new EmployeeInfoDto
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Patronymic = e.Patronymic,
                Email = e.Email,
                Projects = e.EmployeeProjects.Select(ep => new ProjectItemDto
                {
                    Id = ep.Project.Id,
                    Title = ep.Project.Title,
                    CompanyCustomer = ep.Project.CompanyCustomer,
                    CompanyExecuter = ep.Project.CompanyExecutor,
                    StartDate = ep.Project.StartDate,
                    FinishDate = ep.Project.FinishDate,
                    Priority = ep.Project.Priority
                }).ToList(),
                AuthoredIssues = e.AuthoredIssues.Select(i => new IssueItemDto
                {
                    Id = i.Id,
                    Title = i.Title,
                    Status = i.Status,
                    Comments = i.Comments,
                    Priority = i.Priority
                }).ToList(),
                ExecutedIssues = e.ExecutedIssues.Select(i => new IssueItemDto
                {
                    Id = i.Id,
                    Title = i.Title,
                    Status = i.Status,
                    Comments = i.Comments,
                    Priority = i.Priority
                }).ToList()
            })
            .SingleOrDefaultAsync();
    }

    public async Task<bool> IsEmailExists(string email)
    {
        return await _dbContext.Employees.AnyAsync(e => e.Email.Equals(email));
    }

    public async Task<bool> EmployeeExistsAsync(int id)
    {
        return await _dbContext.Employees
            .Where(e => e.Id == id)
            .AnyAsync();
    }

    public async Task<bool> HasManagedProjects(int employeeId)
    {
        return await _dbContext.Projects
            .AnyAsync(p => p.ProjectManagerId == employeeId);
    }

    public async Task<bool> HasIssues(int employeeId)
    {
        return await _dbContext.Issues.AnyAsync(i =>
            i.AuthorId == employeeId ||
            i.ExecutorId == employeeId);
    }

    public async Task<IReadOnlyList<int>> GetEmployeesWithProjectsAsync(IReadOnlyCollection<int> ids)
    {
        return await _dbContext.EmployeesProjects
            .AsNoTracking()
            .Where(ep => ids.Contains(ep.EmployeeId))
            .Select(ep => ep.EmployeeId)
            .Distinct()
            .ToListAsync();
    }
}