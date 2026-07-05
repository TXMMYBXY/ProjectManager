namespace ProjectManager.Application.Common;

public interface IEmployeeProjectRepository : IBaseRepository<Entities.Models.EmployeeProject>
{
    Task AddAsync(int employeeId, int projectId);
    Task AddRangeToProjectAsync(int projectId, IReadOnlyList<int> employeesIds);
    Task AddRangeToEmployeeAsync(int employeeId, IReadOnlyList<int> projectIds);
    Task<int> RemoveAsync(int employeeId, int projectId);
    Task<int> DeleteRangeProjectsAsync(int employeeId, IReadOnlyList<int> projectIds);
    Task<int> DeleteRangeEmployeesAsync(int projectId, IReadOnlyList<int> employeesIds);
    Task<bool> ExistsAsync(int employeeId, int projectId);
    Task<bool> HasAnyLinksForEmployeeAsync(int employeeId);
    Task<bool> HasAnyLinksForProjectAsync(int projectId);
}