namespace ProjectManager.Api.Features.Employee.Requests;

public class BulkInsertRequest
{
    public IReadOnlyList<int> Ids { get; set; }
}