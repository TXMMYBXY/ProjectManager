namespace ProjectManager.Entities.Models;

public class EmployeeProject : EntityBase
{
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;
    
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
}