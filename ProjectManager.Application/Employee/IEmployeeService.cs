using ProjectManager.Application.Employee.Dto;

namespace ProjectManager.Application.Employee;

public interface IEmployeeService
{
    Task<PagedEmployeeDto> GetAllEmployeesAsync(EmployeeFilter filter);
    Task<EmployeeInfoDto> GetEmployeeByIdAsync(int id);
    Task CreateEmployeeAsync(CreateEmployeeDto dto);
    Task<EmployeeInfoDto> UpdateEmployeeAsync(int employeeId, UpdateEmployeeDto dto);
    Task<bool> DeleteEmployeeByIdAsync(int id);
    Task<int> BulkDeleteEmployeesAsync(IReadOnlyList<int> ids);
}