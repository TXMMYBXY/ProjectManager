using System.Linq.Expressions;
using ProjectManager.Application.Common;
using ProjectManager.Application.Employee.Dto;

namespace ProjectManager.Application.Employee;

public interface IEmployeeRepository : IBaseRepository<Entities.Models.Employee>
{
    Task<(IReadOnlyList<EmployeeItemDto> Employees, int Count)> GetAllEmployeesAsync(EmployeeFilter filter, 
        Expression<Func<Entities.Models.Employee, bool>>? predicate = null);
    Task<EmployeeInfoDto?> GetEmployeeByIdAsync(int employeeId, 
        Expression<Func<Entities.Models.Employee, bool>>? predicate = null);
    Task<bool> IsEmailExists(string email);
    Task<bool> EmployeeExistsAsync(int id);
    Task<bool> HasManagedProjects(int employeeId);
    Task<bool> HasIssues(int employeeId);
    Task<IReadOnlyList<int>> GetEmployeesWithProjectsAsync(IReadOnlyCollection<int> ids);
}