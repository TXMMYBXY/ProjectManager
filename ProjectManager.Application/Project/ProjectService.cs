using System.Linq.Expressions;
using AutoMapper;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Common;
using ProjectManager.Application.Common.Exceptions;
using ProjectManager.Application.Project.Dto;
using ProjectManager.Application.Utils;
using ProjectManager.Entities.Enums;

namespace ProjectManager.Application.Project;

public class ProjectService : IProjectService
{
    private readonly ILogger<ProjectService> _logger;
    private readonly IMapper _mapper;
    private readonly IProjectRepository _projectRepository;
    private readonly IEmployeeProjectRepository _employeeProjectRepository;

    public ProjectService(
        ILogger<ProjectService> logger, 
        IMapper mapper,
        IProjectRepository projectRepository,
        IEmployeeProjectRepository employeeProjectRepository)
    {
        _logger = logger;
        _mapper = mapper;
        _projectRepository = projectRepository;
        _employeeProjectRepository = employeeProjectRepository;
    }
    
    public async Task<PagedProjectDto> GetAllProjectsAsync(ProjectFilter filter, CurrentUser currentUser)
    {
        Expression<Func<Entities.Models.Project, bool>>? predicate = null;

        if (currentUser.Role.Equals(nameof(UserRole.Manager)))
        {
            predicate = p => p.ProjectManagerId == currentUser.Id;
        }
        else if (currentUser.Role.Equals(nameof(UserRole.Employee)))
        {
            predicate = p => p.EmployeeProjects.Any(ep => ep.EmployeeId == currentUser.Id);
        }

        var projectsDto = await _projectRepository.GetAllProjectsAsync(filter, predicate);
        
        return new PagedProjectDto
        {
            Projects = projectsDto.Projects,
            TotalCount = projectsDto.Count,
            PageSize = filter.PageSize ?? projectsDto.Count,
            CurrentPage = filter.PageNumber ?? 1
        };
    }

    public async Task<ProjectInfoDto> GetProjectByIdAsync(int id, CurrentUser currentUser)
    {
        Expression<Func<Entities.Models.Project, bool>>? predicate = null;
        
        if (currentUser.Role.Equals(nameof(UserRole.Manager)))
        {
            predicate = p => p.ProjectManagerId == currentUser.Id;
        }
        
        var projectDto = await _projectRepository.GetProjectByIdAsync(id, predicate);
        
        NotFoundException.ThrowIfNull(projectDto, "Project not found");
        
        return projectDto;
    }

    public async Task CreateProjectAsync(CreateProjectDto dto)
    {
        var project = _mapper.Map<Entities.Models.Project>(dto);

        // If ProjectManager navigation property is not populated by mapper, check ProjectManagerId
        ConflictException.ThrowIf(project.ProjectManager == null && project.ProjectManagerId == 0, "Project manager not found");
        
        await _projectRepository.CreateAsync(project);
        await _projectRepository.SaveChangesAsync();
        
        _logger.LogInformation("Project successfully created with id {0}", project.Id);
    }

    public async Task<ProjectInfoDto> AssignEmployeeToProjectAsync(int employeeId, int projectId)
    {
        var exists = await _employeeProjectRepository.ExistsAsync(employeeId, projectId);
        
        ConflictException.ThrowIf(exists, "Employee already assigned to this project");
        
        await _employeeProjectRepository.AddAsync(employeeId, projectId);
        await _employeeProjectRepository.SaveChangesAsync();
        
        var projectDto = await _projectRepository.GetProjectByIdAsync(projectId);

        return projectDto;
    }

    public async Task<int> BulkInsertEmployeesToProjectAsync(IReadOnlyList<int> employeeIds, int projectId)
    {
        await _employeeProjectRepository.AddRangeToProjectAsync(projectId, employeeIds);
        await _employeeProjectRepository.SaveChangesAsync();
        
        _logger.LogInformation("Bulk insert employees successfully");
        
        return employeeIds.Count;
    }

    public async Task<ProjectInfoDto> UpdateProjectAsync(int projectId, UpdateProjectDto dto)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);
        
        NotFoundException.ThrowIfNull(project, "Project not found");

        // Map fields except FinishDate (Optional) to avoid AutoMapper mapping issues in tests
        if (dto.Title != null)
            project.Title = dto.Title;
        if (dto.CompanyCustomer != null)
            project.CompanyCustomer = dto.CompanyCustomer;
        if (dto.CompanyExecutor != null)
            project.CompanyExecutor = dto.CompanyExecutor;
        if (dto.Priority.HasValue)
            project.Priority = dto.Priority.Value;
        if (dto.ProjectManagerId.HasValue)
            project.ProjectManagerId = dto.ProjectManagerId.Value;

        if (dto.FinishDate.HasValue)
            project.FinishDate = dto.FinishDate.Value;
        
        await _projectRepository.SaveChangesAsync();
        
        _logger.LogInformation("Project successfully updated with id {0}", projectId);
        
        return _mapper.Map<ProjectInfoDto>(project);
    }

    public async Task<bool> DeleteProjectByIdAsync(int id)
    {
        var hasLinks = await _employeeProjectRepository.HasAnyLinksForProjectAsync(id);
        ConflictException.ThrowIf(hasLinks, "Project has linked employees. Delete impossible");
        
        var result = await _projectRepository.DeleteByIdAsync(id) > 0;

        if(result)
            _logger.LogInformation("Project successfully deleted with id {0}", id);
        else
            throw new NotFoundException("Project not found");
        
        return result;
    }

    public async Task<int> BulkDeleteProjectsAsync(IReadOnlyList<int> ids)
    {
        var result = await _projectRepository.BulkDeleteAsync(ids);

        _logger.LogInformation("Bulk delete projects successfully");

        return result;
    }

    public async Task<bool> DeleteEmployeeFromProjectAsync(int projectId, int employeeId)
    {
        var result = await _employeeProjectRepository.RemoveAsync(employeeId, projectId) > 0;
        
        if (result)
            _logger.LogInformation("Employee {0} unlinked from project {1} successfully", employeeId, projectId);
        else
            throw new NotFoundException("Employee or project not found");
        
        return result;
    }

    public async Task<int> BulkDeleteEmployeesFromProjectAsync(IReadOnlyList<int> employeesIds, int projectId)
    {
        var exists = await _projectRepository.ProjectExistsAsync(projectId);
        
        ConflictException.ThrowIf(!exists, "Project not found");
        
        return await _employeeProjectRepository.DeleteRangeEmployeesAsync(projectId, employeesIds);
    }
}