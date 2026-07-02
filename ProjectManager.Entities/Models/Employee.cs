using ProjectManager.Entities.Enums;

namespace ProjectManager.Entities.Models;

public class Employee : EntityBase
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? Patronymic { get; set; }
    public string Email { get; set; }
    
    public UserRole Role { get; set; } = UserRole.Employee;
    
    public ICollection<Project> ManagedProjects { get; set; } = new List<Project>();
    public ICollection<EmployeeProject> EmployeeProjects { get; set; } = new List<EmployeeProject>();
    public ICollection<Issue> AuthoredIssues { get; set; } = new List<Issue>();
    public ICollection<Issue> ExecutedIssues { get; set; } = new List<Issue>();
}