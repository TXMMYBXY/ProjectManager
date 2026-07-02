using ProjectManager.Application.Employee.Dto;

namespace ProjectManager.Application.Project.Dto;

public class ProjectSummaryDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string CompanyCustomer { get; set; }
    public string CompanyExecuter { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? FinishDate { get; set; }
    public byte Priority { get; set; } = 1;
}