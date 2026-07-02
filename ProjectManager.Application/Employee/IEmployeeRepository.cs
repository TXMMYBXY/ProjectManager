using ProjectManager.Application.Common;
using ProjectManager.Application.Employee.Dto;

namespace ProjectManager.Application.Employee;

public interface IEmployeeRepository : IBaseRepository<Entities.Models.Employee>
{
    Task<(ICollection<EmployeeItemDto>, int)> GetAllEmployeesAsync(EmployeeFilter filter);
    Task<EmployeeInfoDto?> GetEmployeeByIdAsync(int employeeId);
}