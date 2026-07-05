using ProjectManager.Application.Project.Dto;

namespace ProjectManager.Application.Project;

public interface IProjectService
{
    Task<PagedProjectDto> GetAllProjectsAsync(ProjectFilter filter);
    Task<ProjectInfoDto> GetProjectByIdAsync(int id);
    Task CreateProjectAsync(CreateProjectDto dto);
    Task<ProjectInfoDto> AssignEmployeeToProjectAsync(int employeeId, int projectId);
    Task<int> BulkInsertEmployeesToProjectAsync(IReadOnlyList<int> employeeIds, int projectId);
    Task<ProjectInfoDto> UpdateProjectAsync(int projectId, UpdateProjectDto dto);
    Task<bool> DeleteProjectByIdAsync(int id);
    Task<int> BulkDeleteProjectsAsync(IReadOnlyList<int> ids);
    Task<bool> DeleteEmployeeFromProjectAsync(int projectId, int employeeId);
    Task<int> BulkDeleteEmployeesFromProjectAsync(IReadOnlyList<int> employeesIds, int projectId);
}