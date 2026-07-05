using System.Text.Json.Serialization;
using ProjectManager.Application.Issue;
using ProjectManager.Entities.Enums;

namespace ProjectManager.Api.Features.Issue.Requests;

public class GetIssuesRequest
{
    [JsonPropertyName("sortField")]
    public IssueSortField? SortField { get; set; }
    [JsonPropertyName("descending")]
    public bool Descending { get; set; }
    
    [JsonPropertyName("title")]
    public string? Title { get; set; }
    [JsonPropertyName("status")]
    public IssueStatus? Status { get; set; }
    [JsonPropertyName("priority")]
    public byte? Priority { get; set; }
    [JsonPropertyName("projectTitle")]
    public string? ProjectTitle { get; set; }
    [JsonPropertyName("authorFullName")]
    public string? AuthorFullName { get; set; }
    [JsonPropertyName("executorFullName")]
    public string? ExecutorFullName { get; set; }
    
    [JsonPropertyName("pageSize")]
    public int? PageSize { get; set; }
    [JsonPropertyName("pageNumber")]
    public int? PageNumber { get; set; }
}