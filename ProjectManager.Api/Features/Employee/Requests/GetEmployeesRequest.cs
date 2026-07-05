using System.Text.Json.Serialization;
using ProjectManager.Application.Employee;

namespace ProjectManager.Api.Features.Employee.Requests;

public class GetEmployeesRequest
{
    [JsonPropertyName("sortField")]
    public EmployeeSortField? SortField { get; set; }
    [JsonPropertyName("descending")]
    public bool Descending { get; set; }
    
    [JsonPropertyName("firstName")]
    public string? FirstName { get; set; }
    [JsonPropertyName("lastName")]
    public string? LastName { get; set; }
    [JsonPropertyName("patronymic")]
    public string? Patronymic { get; set; }
    [JsonPropertyName("email")]
    public string? Email { get; set; }
    
    [JsonPropertyName("pageSize")]
    public int? PageSize { get; set; }
    [JsonPropertyName("pageNumber")]
    public int? PageNumber { get; set; }
}