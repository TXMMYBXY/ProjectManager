using System.Text.Json.Serialization;
using ProjectManager.Application.Issue.Dto;

namespace ProjectManager.Api.Features.Issue.Responses;

public class PagedIssueResponse
{
    [JsonPropertyName("issues")]
    public IReadOnlyList<IssueItemDto>? Issues { get; set; }
    
    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }

    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; }

    [JsonPropertyName("currentPage")]
    public int CurrentPage { get; set; }

    [JsonPropertyName("totalPages")]
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}