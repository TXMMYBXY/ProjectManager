using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ProjectManager.Api.Features.Project.Requests;

public class CreateProjectRequest
{
    [Required]
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
    [Required]
    [JsonPropertyName("companyCustomer")]
    public string CompanyCustomer { get; set; } = string.Empty;
    [Required]
    [JsonPropertyName("companyExecutor")]
    public string CompanyExecutor { get; set; } = string.Empty;
    [Required]
    [JsonPropertyName("startDate")]
    public DateTime StartDate { get; set; }
    
    [JsonPropertyName("finishDate")]
    public DateTime? FinishDate { get; set; }
    
    [JsonPropertyName("priority")]
    public byte Priority { get; set; }
    
    [JsonPropertyName("projectManagerId")]
    public int? ProjectManagerId { get; set; }
}