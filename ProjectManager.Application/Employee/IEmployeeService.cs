using ProjectManager.Application.Employee.Dto;
using ProjectManager.Application.Utils;

namespace ProjectManager.Application.Employee;

public interface IEmployeeService
{
    Task<PagedEmployeeDto> GetAllEmployeesAsync(EmployeeFilter filter, CurrentUser currentUser);
    Task<EmployeeInfoDto> GetEmployeeByIdAsync(int id, CurrentUser currentUser);
    Task CreateEmployeeAsync(CreateEmployeeDto dto);
    Task<EmployeeInfoDto> AddProjectToEmployeeAsync(int projectId, int employeeId);
    Task<int> BulkInsertProjectsToEmployeeAsync(IReadOnlyList<int> projectIds, int employeeId);
    Task<EmployeeInfoDto> UpdateEmployeeAsync(int employeeId, UpdateEmployeeDto dto);
    Task<bool> DeleteEmployeeByIdAsync(int id);
    Task<int> BulkDeleteEmployeesAsync(IReadOnlyList<int> ids);
    Task<bool> DeleteProjectFromEmployeeAsync(int projectId, int employeeId);
    Task<int> BulkDeleteProjectsFromEmployeeAsync(IReadOnlyList<int> projectIds, int employeeId);
    Task<ProjectManagerDto> GetProjectManagersAsync();
}