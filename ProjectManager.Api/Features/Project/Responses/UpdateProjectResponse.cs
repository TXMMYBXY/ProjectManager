using System.Text.Json.Serialization;

namespace ProjectManager.Api.Features.Project.Responses;

public class UpdateProjectResponse
{
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
    [JsonPropertyName("projectManagerId")]
    public int? ProjectManagerId { get; set; }
}