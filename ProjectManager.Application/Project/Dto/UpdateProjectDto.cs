namespace ProjectManager.Application.Project.Dto;

public class UpdateProjectDto
{
    public string? Title { get; set; }
    public string? CompanyCustomer { get; set; } 
    public string? CompanyExecutor { get; set; }
    public DateTime FinishDate { get; set; }
    public byte? Priority { get; set; }
    public int? ProjectManagerId { get; set; }
}