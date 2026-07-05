using ProjectManager.Application.Employee.Dto;
using ProjectManager.Application.Issue.Dto;

namespace ProjectManager.Api.Features.Project.Responses;

public class ProjectInfoResponse
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string CompanyCustomer { get; set; }
    public string CompanyExecuter { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? FinishDate { get; set; }
    public byte Priority { get; set; }
    public EmployeeSummaryDto ProjectManager { get; set; }
    public IReadOnlyList<EmployeeItemDto> Employees { get; set; }
    public IReadOnlyList<IssueItemDto> Issues { get; set; }
}