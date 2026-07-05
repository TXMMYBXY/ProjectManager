using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ProjectManager.Entities.Enums;

namespace ProjectManager.Api.Features.Issue.Requests;

public class CreateIssueRequest
{
    [Required]
    [JsonPropertyName("title")]
    public string Title { get; set; }
    [JsonPropertyName("status")]
    public IssueStatus Status { get; set; }
    [JsonPropertyName("comments")]
    public string? Comments { get; set; }
    [JsonPropertyName("priority")]
    public byte Priority { get; set; }
    [Required]
    [JsonPropertyName("projectId")]
    public int ProjectId { get; set; }
    [Required]
    [JsonPropertyName("authorId")]
    public int AuthorId { get; set; }
    [Required]
    [JsonPropertyName("executorId")]
    public int ExecutorId { get; set; }
}