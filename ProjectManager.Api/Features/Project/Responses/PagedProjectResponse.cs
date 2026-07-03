using System.Text.Json.Serialization;
using ProjectManager.Application.Project.Dto;

namespace ProjectManager.Api.Features.Project.Responses;

public class PagedProjectResponse
{
    [JsonPropertyName("users")]
    public IReadOnlyList<ProjectItemDto>? Projects { get; set; }
    
    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }

    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; }

    [JsonPropertyName("currentPage")]
    public int CurrentPage { get; set; }

    [JsonPropertyName("totalPages")]
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}