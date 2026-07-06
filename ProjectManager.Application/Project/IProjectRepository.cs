using System.Linq.Expressions;
using ProjectManager.Application.Common;
using ProjectManager.Application.Project.Dto;

namespace ProjectManager.Application.Project;

public interface IProjectRepository : IBaseRepository<Entities.Models.Project>
{
    Task<(IReadOnlyList<ProjectItemDto> Projects, int Count)> GetAllProjectsAsync(ProjectFilter filter, 
        Expression<Func<Entities.Models.Project, bool>>? predicate = null);
    Task<ProjectInfoDto?> GetProjectByIdAsync(int projectId, Expression<Func<Entities.Models.Project, bool>>? predicate = null);
    Task<bool> ProjectExistsAsync(int id);
    Task<bool> HasManagers(int id);
    Task<bool> HasIssues(int id);
}