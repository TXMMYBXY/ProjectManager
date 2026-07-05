using Microsoft.EntityFrameworkCore;
using ProjectManager.Application.Common;
using ProjectManager.Entities.Models;
using ProjectManager.Infrastructure.Data;

namespace ProjectManager.Infrastructure.Common;

public class EmployeeProjectRepository : BaseRepository<EmployeeProject>, IEmployeeProjectRepository
{
    private readonly ApplicationDbContext _dbContext;
    
    public EmployeeProjectRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<int>> GetEmployeeIdsByProjectAsync(int projectId)
    {
        return await _dbContext.EmployeesProjects
            .Where(ep => ep.ProjectId == projectId)
            .Select(ep => ep.EmployeeId)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<int>> GetProjectIdsByEmployeeAsync(int employeeId)
    {
        return await _dbContext.EmployeesProjects
            .Where(ep => ep.EmployeeId == employeeId)
            .Select(ep => ep.ProjectId)
            .ToListAsync();
    }

    public async Task AddAsync(int employeeId, int projectId)
    {
        await _dbContext.EmployeesProjects.AddAsync(new EmployeeProject
        {
            EmployeeId = employeeId,
            ProjectId = projectId
        });
    }

    public async Task AddRangeToProjectAsync(int projectId, IReadOnlyList<int> employeesIds)
    {
        var existing = await _dbContext.EmployeesProjects
            .Where(x => x.ProjectId == projectId && employeesIds.Contains(x.EmployeeId))
            .Select(x => x.EmployeeId)
            .ToListAsync();

        var newItems = employeesIds
            .Except(existing)
            .Select(id => new EmployeeProject
            {
                EmployeeId = id,
                ProjectId = projectId
            });

        await _dbContext.EmployeesProjects.AddRangeAsync(newItems);
    }

    public async Task AddRangeToEmployeeAsync(int employeeId, IReadOnlyList<int> projectIds)
    {
        var existing = await _dbContext.EmployeesProjects
            .Where(x => x.EmployeeId == employeeId && projectIds.Contains(x.ProjectId))
            .Select(x => x.ProjectId)
            .ToListAsync();

        var newItems = projectIds
            .Except(existing)
            .Select(id => new EmployeeProject
            {
                EmployeeId = employeeId,
                ProjectId = id
            });

        await _dbContext.EmployeesProjects.AddRangeAsync(newItems);
    }

    public async Task<int> RemoveAsync(int employeeId, int projectId)
    {
        return await _dbContext.EmployeesProjects
            .Where(j => j.EmployeeId == employeeId && j.ProjectId == projectId)
            .ExecuteDeleteAsync();
    }

    public async Task<int> DeleteRangeProjectsAsync(int employeeId, IReadOnlyList<int> projectIds)
    {
        return await _dbContext.EmployeesProjects
            .Where(ep => ep.EmployeeId == employeeId && projectIds.Contains(ep.ProjectId))
            .ExecuteDeleteAsync();
    }

    public async Task<int> DeleteRangeEmployeesAsync(int projectId, IReadOnlyList<int> employeesIds)
    {
        return await _dbContext.EmployeesProjects
            .Where(ep => ep.ProjectId == projectId && employeesIds.Contains(ep.EmployeeId))
            .ExecuteDeleteAsync();
    }

    public async Task<bool> ExistsAsync(int employeeId, int projectId)
    {
        return await  _dbContext.EmployeesProjects
            .AnyAsync(ep => ep.EmployeeId == employeeId && ep.ProjectId == projectId);
    }

    public async Task<bool> HasAnyLinksForEmployeeAsync(int employeeId)
    {
        return await _dbContext.EmployeesProjects
            .AnyAsync(ep => ep.EmployeeId == employeeId);
    }

    public async Task<bool> HasAnyLinksForProjectAsync(int projectId)
    {
        return await _dbContext.EmployeesProjects
            .AnyAsync(ep => ep.ProjectId == projectId);
    }

    public Task<bool> HasLinkedProjectsAsync(IReadOnlyCollection<int> employeeIds)
    {
        return _dbContext.EmployeesProjects
            .AnyAsync(ep => employeeIds.Contains(ep.EmployeeId));
    }
}