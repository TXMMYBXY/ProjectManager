using AutoMapper;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Common.Exceptions;
using ProjectManager.Application.Employee;
using ProjectManager.Application.Employee.Dto;

namespace ProjectManager.Infrastructure.Employee;

public class EmployeeService : IEmployeeService
{
    private readonly ILogger<EmployeeService> _logger;
    private readonly  IMapper _mapper;
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeeService(
        ILogger<EmployeeService> logger,
        IMapper mapper,
        IEmployeeRepository employeeRepository)
    {
        _logger = logger;
        _mapper = mapper;
        _employeeRepository = employeeRepository;
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
        
        await _employeeRepository.SaveChangesAsync();
        
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
}