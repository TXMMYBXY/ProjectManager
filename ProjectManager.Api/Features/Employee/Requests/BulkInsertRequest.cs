using System.Text.Json.Serialization;

namespace ProjectManager.Api.Features.Employee.Requests;

public class BulkInsertRequest
{
    [JsonPropertyName("ids")]
    public IReadOnlyList<int> Ids { get; set; }
}