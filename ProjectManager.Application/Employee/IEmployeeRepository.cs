using ProjectManager.Application.Common;
using ProjectManager.Application.Employee.Dto;

namespace ProjectManager.Application.Employee;

public interface IEmployeeRepository : IBaseRepository<Entities.Models.Employee>
{
    Task<(IReadOnlyList<EmployeeItemDto> Employees, int Count)> GetAllEmployeesAsync(EmployeeFilter filter);
    Task<EmployeeInfoDto?> GetEmployeeByIdAsync(int employeeId);
}