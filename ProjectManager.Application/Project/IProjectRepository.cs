using ProjectManager.Application.Common;
using ProjectManager.Application.Project.Dto;

namespace ProjectManager.Application.Project;

public interface IProjectRepository : IBaseRepository<Entities.Models.Project>
{
    Task<(ICollection<ProjectItemDto>, int)> GetAllProjectsAsync(ProjectFilter filter);
    Task<ProjectInfoDto?> GetProjectByIdAsync(int projectId);
}