using System.Linq.Expressions;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Common;
using ProjectManager.Application.Common.Exceptions;
using ProjectManager.Application.Employee;
using ProjectManager.Application.Employee.Dto;
using ProjectManager.Entities.Enums;

namespace ProjectManager.Infrastructure.Employee;

public class EmployeeService : IEmployeeService
{
    private readonly ILogger<EmployeeService> _logger;
    private readonly IMapper _mapper;
    ICurrentUser _currentUser;
    private readonly UserManager<Entities.Models.Employee> _userManager;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IEmployeeProjectRepository _employeeProjectRepository;

    public EmployeeService(
        ILogger<EmployeeService> logger,
        IMapper mapper,
        ICurrentUser currentUser,
        UserManager<Entities.Models.Employee> userManager,
        IEmployeeRepository employeeRepository,
        IEmployeeProjectRepository employeeProjectRepository)
    {
        _logger = logger;
        _mapper = mapper;
        _currentUser = currentUser;
        _userManager = userManager;
        _employeeRepository = employeeRepository;
        _employeeProjectRepository = employeeProjectRepository;
    }
    
    public async Task<PagedEmployeeDto> GetAllEmployeesAsync(EmployeeFilter filter)
    {
        Expression<Func<Entities.Models.Employee, bool>>? predicate = null;

        if (_currentUser.IsInRole(nameof(UserRole.Manager)))
        {
            predicate = e =>
                e.EmployeeProjects.Select(ep => ep.Project).Where(pr => pr.ProjectManagerId == _currentUser.Id).Any();
        }
        else if (_currentUser.IsInRole(nameof(UserRole.Employee)))
        {
            predicate = p => p.Id == _currentUser.Id;
        }

        var employeesDto = await _employeeRepository.GetAllEmployeesAsync(filter, predicate);

        return new PagedEmployeeDto
        {
            Employees = employeesDto.Employees,
            TotalCount = employeesDto.Count,
            PageSize = filter.PageSize ?? employeesDto.Count,
            CurrentPage = filter.PageNumber ?? 1
        };
    }

    public async Task<EmployeeInfoDto> GetEmployeeByIdAsync(int id)
    {
        Expression<Func<Entities.Models.Employee, bool>>? predicate = null;

        if (_currentUser.IsInRole(nameof(UserRole.Manager)))
        {
            predicate = e => e.EmployeeProjects.Where(ep => ep.Project.ProjectManagerId == _currentUser.Id).Any();
        }

        var employeeDto = await _employeeRepository.GetEmployeeByIdAsync(id, predicate);
        
        NotFoundException.ThrowIfNull(employeeDto, "Employee not found");
        
        return employeeDto;
    }

    public async Task CreateEmployeeAsync(CreateEmployeeDto dto)
    {
        var employee = _mapper.Map<Entities.Models.Employee>(dto);

        ConflictException.ThrowIf(await _employeeRepository.IsEmailExists(dto.Email), "Email already exists");

        employee.UserName = dto.Email;

        if (_userManager == null)
        {
            await _employeeRepository.CreateAsync(employee);
            await _employeeRepository.SaveChangesAsync();
            _logger.LogInformation("Employee successfully created with id {0}", employee.Id);
            return;
        }

        var result = await _userManager.CreateAsync(employee, dto.Password);

        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(employee, dto.Role.ToString());

        _logger.LogInformation("Employee successfully created with id {0}", employee.Id);
    }

    public async Task<EmployeeInfoDto> AddProjectToEmployeeAsync(int projectId, int employeeId)
    {
        var exists = await _employeeProjectRepository.ExistsAsync(employeeId, projectId);
        
        ConflictException.ThrowIf(exists, "Project already assigned to employee");
        
        await _employeeProjectRepository.AddAsync(employeeId, projectId);
        await _employeeProjectRepository.SaveChangesAsync();
        
        var employeeDto = await _employeeRepository.GetEmployeeByIdAsync(employeeId);

        return employeeDto;
    }
    
    public async Task<int> BulkInsertProjectsToEmployeeAsync(IReadOnlyList<int> projectIds, int employeeId)
    {
        var employeeExists = await _employeeRepository.EmployeeExistsAsync(employeeId);
        
        ConflictException.ThrowIf(!employeeExists, "Employee already is not exists");
        
        await _employeeProjectRepository.AddRangeToEmployeeAsync(employeeId, projectIds);
        await _employeeProjectRepository.SaveChangesAsync();
        
        _logger.LogInformation("Bulk insert projects successfully");
        
        return projectIds.Count;
    }

    public async Task<EmployeeInfoDto> UpdateEmployeeAsync(int employeeId, UpdateEmployeeDto dto)
    {
        var employee = await _employeeRepository.GetByIdAsync(employeeId);
        
        NotFoundException.ThrowIfNull(employee, "Employee not found");
        
        _mapper.Map(dto, employee);
        
        if(dto.Patronymic.HasValue)
            employee.Patronymic = dto.Patronymic.Value;

        await _employeeRepository.SaveChangesAsync();
        
        _logger.LogInformation("Employee successfully updated with id {0}", employee.Id);;
        
        return _mapper.Map<EmployeeInfoDto>(employee);
    }

    public async Task<bool> DeleteEmployeeByIdAsync(int id)
    {
        var hasLinks = await _employeeProjectRepository.HasAnyLinksForEmployeeAsync(id);
        ConflictException.ThrowIf(hasLinks, "Employee has linked projects. Delete impossible");

        var hasManagedProjects = await _employeeRepository.HasManagedProjects(id);
        ConflictException.ThrowIf(hasManagedProjects, "Employee has managed projects. Delete impossible");

        var hasIssues = await _employeeRepository.HasIssues(id);
        ConflictException.ThrowIf(hasIssues, "Employee has issues. Delete impossible");
        
        var result = await _employeeRepository.DeleteByIdAsync(id) > 0;
        
        if (result)
            _logger.LogInformation("Employee successfully deleted with id {0}", id);
        else
            throw new NotFoundException("Employee not found");
        
        return result;
    }

    public async Task<int> BulkDeleteEmployeesAsync(IReadOnlyList<int> ids)
    {
        var blocked = await _employeeRepository.GetEmployeesWithProjectsAsync(ids);
        
        ConflictException.ThrowIf(blocked.Count > 0, 
            "Some employees cannot be deleted because they are referenced by other entities");
        
        var result = await _employeeRepository.BulkDeleteAsync(ids);

        await _employeeRepository.SaveChangesAsync();
        
        _logger.LogInformation("Bulk delete employees successfully");

        return result;
    }

    public async Task<bool> DeleteProjectFromEmployeeAsync(int projectId, int employeeId)
    {
        var result = await _employeeProjectRepository.RemoveAsync(employeeId, projectId) > 0;
        
        if (result)
            _logger.LogInformation("Employee {0} unlinked from project {1} successfully", employeeId, projectId);
        else
            throw new NotFoundException("Employee or project not found");
        
        return result;
    }

    public async Task<int> BulkDeleteProjectsFromEmployeeAsync(IReadOnlyList<int> projectIds, int employeeId)
    {
        var exists = await _employeeRepository.EmployeeExistsAsync(employeeId);

        ConflictException.ThrowIf(!exists, "Employee not found");
        
        return await _employeeProjectRepository.DeleteRangeProjectsAsync(employeeId, projectIds);
    }

    public async Task<ProjectManagerDto> GetProjectManagersAsync()
    {
        var managers = await _userManager.GetUsersInRoleAsync(UserRole.Manager.ToString());
        var directors = await _userManager.GetUsersInRoleAsync(UserRole.Director.ToString());
        
        var all = managers.Concat(directors);

        var response = new ProjectManagerDto
        {
            ProjectManagers = _mapper.Map<IReadOnlyList<EmployeeItemDto>>(managers.Concat(directors))
        };

        return response;
    }
}