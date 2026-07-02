using ProjectManager.Entities.Enums;

namespace ProjectManager.Entities.Models;

public class Issue : EntityBase
{
    public required string Title { get; set; }
    
    public IssueStatus Status { get; set; } = IssueStatus.ToDo;
    
    public string? Comments { get; set; }

    public byte Priority { get; set; } = 1;
    
    public int ProjectId { get; set; }
    public Project Project { get; set; }
    
    public int AuthorId { get; set; }
    public Employee Author { get; set; }
    
    public int ExecutorId { get; set; }
    public Employee Executor { get; set; }
}