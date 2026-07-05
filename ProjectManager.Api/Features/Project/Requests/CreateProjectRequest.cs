
using System.ComponentModel.DataAnnotations;

namespace ProjectManager.Api.Features.Project.Requests;

public class CreateProjectRequest
{
    [Required]
    public string Title { get; set; }
    [Required]
    public string CompanyCustomer { get; set; }
    [Required]
    public string CompanyExecutor { get; set; }
    [Required]
    public DateTime StartDate { get; set; }
    
    public DateTime? FinishDate { get; set; }
    
    public byte Priority { get; set; }
    
    public int? ProjectManagerId { get; set; }
}