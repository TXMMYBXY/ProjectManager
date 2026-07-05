using System.ComponentModel.DataAnnotations;
using ProjectManager.Entities.Enums;

namespace ProjectManager.Api.Features.Issue.Requests;

public class CreateIssueRequest
{
    [Required]
    public string Title { get; set; }
    public IssueStatus Status { get; set; }
    public string? Comments { get; set; }
    public byte Priority { get; set; }
    [Required]
    public int ProjectId { get; set; }
    [Required]
    public int AuthorId { get; set; }
    [Required]
    public int ExecutorId { get; set; }
}