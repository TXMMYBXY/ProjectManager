using ProjectManager.Application.Project.Dto;

namespace ProjectManager.Application.Project;

public interface IProjectService
{
    Task<PagedProjectDto> GetAllProjectsAsync(ProjectFilter filter);
    Task<ProjectInfoDto> GetProjectByIdAsync(int id);
    Task CreateProjectAsync(CreateProjectDto dto);
    Task<ProjectInfoDto> UpdateProjectAsync(int projectId, UpdateProjectDto dto);
    Task<bool> DeleteProjectByIdAsync(int id);
    Task<int> BulkDeleteProjectsAsync(IReadOnlyCollection<int> ids);
}