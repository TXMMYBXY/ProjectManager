using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectManager.Entities.Models;

public class Project : EntityBase
{
    public string Title { get; set; }
    
    public string CompanyCustomer { get; set; }
    
    public string CompanyExecuter { get; set; }

    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime? FinishDate { get; set; }

    public byte Priority { get; set; } = 1;
    
    public int ProjectManagerId { get; set; }
    public Employee ProjectManager { get; set; }
    
    public ICollection<Issue> Issues { get; set; }
    
    public ICollection<EmployeeProject> EmployeeProjects { get; set; }
}