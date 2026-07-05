using ProjectManager.Application.Project;

namespace ProjectManager.Api.Features.Project.Requests;

public class GetProjectsRequest
{
    public ProjectSortField? SortBy { get; set; }
    public bool Descending { get; set; }
    
    public string? Title { get; set; }
    public string? CompanyCustomer { get; set; }
    public string? CompanyExecuter { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
    public DateTime? FinishDateFrom { get; set; }
    public DateTime? FinishDateTo { get; set; }
    public byte? Priority { get; set; }
    public int? ProjectManagerId { get; set; }

    public int? PageSize { get; set;}
    public int? PageNumber { get; set; }
}