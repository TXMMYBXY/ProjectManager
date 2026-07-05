namespace ProjectManager.Api.Features.Project.Requests;

public class UpdateProjectRequest
{
    public string? Title { get; set; }
    public string? CompanyCustomer { get; set; }
    public string? CompanyExecuter { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? FinishDate { get; set; }
    public byte? Priority { get; set; }
    public int? ProjectManagerId { get; set; }
}