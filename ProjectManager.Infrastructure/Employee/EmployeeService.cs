using AutoMapper;
using Microsoft.Extensions.Logging;
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
        
        ArgumentNullException.ThrowIfNull(employeeDto, "Employee not found");
        
        return employeeDto;
    }

    public async Task CreateEmployeeAsync(CreateEmployeeDto dto)
    {
        var employee = _mapper.Map<Entities.Models.Employee>(dto);
        
        await _employeeRepository.CreateAsync(employee);
        await _employeeRepository.SaveChangesAsync();
    }

    public async Task<EmployeeInfoDto> UpdateEmployeeAsync(int employeeId, UpdateEmployeeDto dto)
    {
        var employee = await _employeeRepository.GetByIdAsync(employeeId);
        
        ArgumentNullException.ThrowIfNull(employee, "Employee not found");
        
        _mapper.Map(dto, employee);

        await _employeeRepository.SaveChangesAsync();
        
        return _mapper.Map<EmployeeInfoDto>(employee);
    }

    public async Task<bool> DeleteEmployeeByIdAsync(int id)
    {
        return await _employeeRepository.DeleteAsync(id) > 0;
    }

    public async Task<int> BulkDeleteEmployeesAsync(IReadOnlyCollection<int> ids)
    {
        return await _employeeRepository.BulkDeleteAsync(ids);
    }
}