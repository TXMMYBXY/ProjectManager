using System.Text.Json.Serialization;
using ProjectManager.Application.Project;

namespace ProjectManager.Api.Features.Project.Requests;

public class GetProjectsRequest
{
    [JsonPropertyName("sortBy")]
    public ProjectSortField? SortBy { get; set; }
    [JsonPropertyName("descending")]
    public bool Descending { get; set; }
    
    [JsonPropertyName("title")]
    public string? Title { get; set; }
    [JsonPropertyName("companyCustomer")]
    public string? CompanyCustomer { get; set; }
    [JsonPropertyName("companyExecuter")]
    public string? CompanyExecuter { get; set; }
    [JsonPropertyName("startDateFrom")]
    public DateTime? StartDateFrom { get; set; }
    [JsonPropertyName("startDateTo")]
    public DateTime? StartDateTo { get; set; }
    [JsonPropertyName("finishDateFrom")]
    public DateTime? FinishDateFrom { get; set; }
    [JsonPropertyName("finishDateTo")]
    public DateTime? FinishDateTo { get; set; }
    [JsonPropertyName("priority")]
    public byte? Priority { get; set; }
    [JsonPropertyName("projectManagerId")]
    public int? ProjectManagerId { get; set; }

    [JsonPropertyName("pageSize")]
    public int? PageSize { get; set;}
    [JsonPropertyName("pageNumber")]
    public int? PageNumber { get; set; }
}