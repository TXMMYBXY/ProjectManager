using System.Text.Json.Serialization;
using ProjectManager.Application.Utils;
using ProjectManager.Entities.Enums;

namespace ProjectManager.Api.Features.Issue.Requests;

public class UpdateIssueRequest
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }
    [JsonPropertyName("status")]
    public IssueStatus? Status { get; set; }
    [JsonPropertyName("comments")]
    public Optional<string?> Comments { get; set; }
    [JsonPropertyName("priority")]
    public byte? Priority { get; set; }
    [JsonPropertyName("authorId")]
    public int? AuthorId { get; set; }
    [JsonPropertyName("executorId")]
    public int? ExecutorId { get; set; }
}