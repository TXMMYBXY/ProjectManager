using System.Text.Json.Serialization;

namespace ProjectManager.Api.Features.Project.Requests;

public class BulkDeleteRequest
{
    [JsonPropertyName("ids")]
    public IReadOnlyList<int> Ids { get; set; } = [];
}