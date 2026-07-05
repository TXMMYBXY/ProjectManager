using System.Text.Json.Serialization;
using ProjectManager.Application.Employee.Dto;

namespace ProjectManager.Api.Features.Employee.Responses;

public class PagedEmployeeResponse
{
    [JsonPropertyName("employees")]
    public IReadOnlyList<EmployeeItemDto>? Employees { get; set; }
    
    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }

    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; }

    [JsonPropertyName("currentPage")]
    public int CurrentPage { get; set; }

    [JsonPropertyName("totalPages")]
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}