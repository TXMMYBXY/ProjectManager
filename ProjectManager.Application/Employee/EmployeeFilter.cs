namespace ProjectManager.Application.Employee;

public class EmployeeFilter
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