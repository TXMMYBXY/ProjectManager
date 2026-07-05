using ProjectManager.Application.Common;
using ProjectManager.Application.Project.Dto;

namespace ProjectManager.Application.Project;

public interface IProjectRepository : IBaseRepository<Entities.Models.Project>
{
    Task<(IReadOnlyList<ProjectItemDto> Projects, int Count)> GetAllProjectsAsync(ProjectFilter filter);
    Task<ProjectInfoDto?> GetProjectByIdAsync(int projectId);
    Task<bool> ProjectExistsAsync(int id);
    Task<bool> HasManagers(int id);
    Task<bool> HasIssues(int id);
}