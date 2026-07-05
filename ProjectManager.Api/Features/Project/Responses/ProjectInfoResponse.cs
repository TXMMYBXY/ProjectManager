using System.Text.Json.Serialization;
using ProjectManager.Application.Employee.Dto;
using ProjectManager.Application.Issue.Dto;

namespace ProjectManager.Api.Features.Project.Responses;

public class ProjectInfoResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("title")]
    public string Title { get; set; }
    [JsonPropertyName("companyCustomer")]
    public string CompanyCustomer { get; set; }
    [JsonPropertyName("companyExecuter")]
    public string CompanyExecuter { get; set; }
    [JsonPropertyName("startDate")]
    public DateTime StartDate { get; set; }
    [JsonPropertyName("finishDate")]
    public DateTime? FinishDate { get; set; }
    [JsonPropertyName("priority")]
    public byte Priority { get; set; }
    [JsonPropertyName("projectManager")]
    public EmployeeSummaryDto ProjectManager { get; set; }
    [JsonPropertyName("employees")]
    public IReadOnlyList<EmployeeItemDto> Employees { get; set; }
    [JsonPropertyName("issues")]
    public IReadOnlyList<IssueItemDto> Issues { get; set; }
}