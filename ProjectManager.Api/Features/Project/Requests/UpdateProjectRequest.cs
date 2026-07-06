using System.Text.Json.Serialization;
using ProjectManager.Application.Utils;

namespace ProjectManager.Api.Features.Project.Requests;

public class UpdateProjectRequest
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }
    [JsonPropertyName("companyCustomer")]
    public string? CompanyCustomer { get; set; }
    [JsonPropertyName("companyExecuter")]
    public string? CompanyExecuter { get; set; }
    [JsonPropertyName("finishDate")]
    public Optional<DateTime?> FinishDate { get; set; }
    [JsonPropertyName("priority")]
    public byte? Priority { get; set; }
    [JsonPropertyName("projectManagerId")]
    public int? ProjectManagerId { get; set; }
}