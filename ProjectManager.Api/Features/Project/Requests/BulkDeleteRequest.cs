namespace ProjectManager.Api.Features.Project.Requests;

public class BulkDeleteRequest
{
    public IReadOnlyList<int> Ids { get; set; }
}