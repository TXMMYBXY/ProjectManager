using ProjectManager.Application.Employee;

namespace ProjectManager.Api.Features.Employee.Requests;

public class GetEmployeesRequest
{
    public EmployeeSortField? SortField { get; set; }
    public bool Descending { get; set; }
    
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Patronymic { get; set; }
    public string? Email { get; set; }
    
    public int? PageSize { get; set; }
    public int? PageNumber { get; set; }
}