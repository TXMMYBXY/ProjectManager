using AutoMapper;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Common;
using ProjectManager.Application.Common.Exceptions;
using ProjectManager.Application.Employee;
using ProjectManager.Application.Employee.Dto;

namespace ProjectManager.Infrastructure.Employee;

public class EmployeeService : IEmployeeService
{
    private readonly ILogger<EmployeeService> _logger;
    private readonly  IMapper _mapper;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IEmployeeProjectRepository _employeeProjectRepository;

    public EmployeeService(
        ILogger<EmployeeService> logger,
        IMapper mapper,
        IEmployeeRepository employeeRepository,
        IEmployeeProjectRepository employeeProjectRepository)
    {
        _logger = logger;
        _mapper = mapper;
        _employeeRepository = employeeRepository;
        _employeeProjectRepository = employeeProjectRepository;
    }
    
    public async Task<PagedEmployeeDto> GetAllEmployeesAsync(EmployeeFilter filter)
    {
        var employeesDto = await _employeeRepository.GetAllEmployeesAsync(filter);

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
        var employeeDto = await _employeeRepository.GetEmployeeByIdAsync(id);
        
        NotFoundException.ThrowIfNull(employeeDto, "Employee not found");
        
        return employeeDto;
    }

    public async Task CreateEmployeeAsync(CreateEmployeeDto dto)
    {
        var employee = _mapper.Map<Entities.Models.Employee>(dto);
        
        ConflictException.ThrowIf(await _employeeRepository.IsEmailExists(dto.Email), "Email already exists");
        
        await _employeeRepository.CreateAsync(employee);
        await _employeeRepository.SaveChangesAsync();
        
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

        await _employeeRepository.SaveChangesAsync();
        
        _logger.LogInformation("Employee successfully updated with id {0}", employee.Id);;
        
        return _mapper.Map<EmployeeInfoDto>(employee);
    }

    public async Task<bool> DeleteEmployeeByIdAsync(int id)
    {
        var result = await _employeeRepository.DeleteByIdAsync(id) > 0;
        
        if (result)
            _logger.LogInformation("Employee successfully deleted with id {0}", id);
        else
            throw new NotFoundException("Employee not found");
        
        return result;
    }

    public async Task<int> BulkDeleteEmployeesAsync(IReadOnlyList<int> ids)
    {
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
}