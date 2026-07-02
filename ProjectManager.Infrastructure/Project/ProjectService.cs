using AutoMapper;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Project;
using ProjectManager.Application.Project.Dto;

namespace ProjectManager.Infrastructure.Project;

public class ProjectService : IProjectService
{
    private readonly ILogger<ProjectService> _logger;
    private readonly IMapper _mapper;
    private readonly IProjectRepository _projectRepository;

    public ProjectService(
        ILogger<ProjectService> logger, 
        IMapper mapper,
        IProjectRepository projectRepository)
    {
        _logger = logger;
        _mapper = mapper;
        _projectRepository = projectRepository;
    }
    
    public async Task<PagedProjectDto> GetAllProjectsAsync(ProjectFilter filter)
    {
        var projectsDto = await _projectRepository.GetAllProjectsAsync(filter);

        return new PagedProjectDto
        {
            Projects = projectsDto.Projects,
            TotalCount = projectsDto.Count,
            PageSize = filter.PageSize ?? projectsDto.Count,
            CurrentPage = filter.PageNumber ?? 1
        };
    }

    public async Task<ProjectInfoDto> GetProjectByIdAsync(int id)
    {
        var projectDto = await _projectRepository.GetProjectByIdAsync(id);
        
        ArgumentNullException.ThrowIfNull(projectDto, "Project not found");
        
        return projectDto;
    }

    public async Task CreateProjectAsync(CreateProjectDto dto)
    {
        var project = _mapper.Map<Entities.Models.Project>(dto);
        
        await _projectRepository.CreateAsync(project);
        await _projectRepository.SaveChangesAsync();
    }

    public async Task<ProjectInfoDto> UpdateProjectAsync(int projectId, UpdateProjectDto dto)
    {
        var entity = await _projectRepository.GetByIdAsync(projectId);
        
        ArgumentNullException.ThrowIfNull(entity, "Project not found");

        _mapper.Map(dto, entity);

        await _projectRepository.SaveChangesAsync();
        
        return _mapper.Map<ProjectInfoDto>(entity);
    }

    public async Task<bool> DeleteProjectByIdAsync(int id)
    {
        return await _projectRepository.DeleteAsync(id) > 0;
    }

    public async Task<int> BulkDeleteProjectsAsync(IReadOnlyCollection<int> ids)
    {
        return await _projectRepository.BulkDeleteAsync(ids);
    }
}