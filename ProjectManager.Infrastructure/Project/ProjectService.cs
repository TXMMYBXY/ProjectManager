using AutoMapper;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Common.Exceptions;
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
        
        NotFoundException.ThrowIfNull(projectDto, "Project not found");
        
        return projectDto;
    }

    public async Task CreateProjectAsync(CreateProjectDto dto)
    {
        var project = _mapper.Map<Entities.Models.Project>(dto);
        
        await _projectRepository.CreateAsync(project);
        await _projectRepository.SaveChangesAsync();
        
        _logger.LogInformation("Project successfully created with id {0}", project.Id);
    }

    public async Task<ProjectInfoDto> UpdateProjectAsync(int projectId, UpdateProjectDto dto)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);
        
        NotFoundException.ThrowIfNull(project, "Project not found");

        _mapper.Map(dto, project);

        await _projectRepository.SaveChangesAsync();
        
        _logger.LogInformation("Project successfully updated with id {0}", projectId);
        
        return _mapper.Map<ProjectInfoDto>(project);
    }

    public async Task<bool> DeleteProjectByIdAsync(int id)
    {
        var result = await _projectRepository.DeleteByIdAsync(id) > 0;

        await _projectRepository.SaveChangesAsync();
        
        if(result)
            _logger.LogInformation("Project successfully deleted with id {0}", id);
        else
            throw new NotFoundException("Project not found");
        
        return result;
    }

    public async Task<int> BulkDeleteProjectsAsync(IReadOnlyCollection<int> ids)
    {
        var result = await _projectRepository.BulkDeleteAsync(ids);

        await _projectRepository.SaveChangesAsync();
        
        _logger.LogInformation("Bulk delete projects successfully");

        return result;
    }
}