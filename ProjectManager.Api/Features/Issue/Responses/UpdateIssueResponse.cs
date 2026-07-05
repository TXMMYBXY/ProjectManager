using System.Text.Json.Serialization;
using ProjectManager.Entities.Enums;

namespace ProjectManager.Api.Features.Issue.Responses;

public class UpdateIssueResponse
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }
    [JsonPropertyName("status")]
    public IssueStatus? Status { get; set; }
    [JsonPropertyName("comments")]
    public string? Comments { get; set; }
    [JsonPropertyName("priority")]
    public byte? Priority = 1;
    [JsonPropertyName("authorId")]
    public int? AuthorId { get; set; }
    [JsonPropertyName("executorId")]
    public int? ExecutorId { get; set; }
}