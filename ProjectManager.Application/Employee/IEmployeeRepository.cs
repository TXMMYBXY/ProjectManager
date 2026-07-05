using ProjectManager.Application.Common;
using ProjectManager.Application.Employee.Dto;

namespace ProjectManager.Application.Employee;

public interface IEmployeeRepository : IBaseRepository<Entities.Models.Employee>
{
    Task<(IReadOnlyList<EmployeeItemDto> Employees, int Count)> GetAllEmployeesAsync(EmployeeFilter filter);
    Task<EmployeeInfoDto?> GetEmployeeByIdAsync(int employeeId);
    Task<bool> IsEmailExists(string email);
    Task<bool> EmployeeExistsAsync(int id);
    Task<bool> HasManagedProjects(int employeeId);
    Task<bool> HasIssues(int employeeId);
    Task<IReadOnlyList<int>> GetEmployeesWithProjectsAsync(IReadOnlyCollection<int> ids);
    Task<Entities.Models.Employee?> GetEntityByEmail(string email);
}